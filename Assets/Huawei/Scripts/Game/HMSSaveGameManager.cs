using HuaweiMobileServices.Base;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HmsPlugin
{
    public class HMSSaveGameManager : HMSManagerSingleton<HMSSaveGameManager>
    {
        public IArchivesClient playersClient { get; set; }
        public Action<ArchiveSummary> SelectedAction { get; set; }
        public Action<bool> AddAction { get; set; }

        //public AuthAccount HuaweiId
        //{
        //    get; set;
        //}

        public void SavedGameAuth()
        {
            //HuaweiId = HMSAccountManager.Instance.HuaweiId;
        }
        public IArchivesClient GetArchivesClient()
        {
            if (playersClient == null /*&& HuaweiId != null*/)
            {
                playersClient = Games.GetArchiveClient();
                Debug.Log("[HMSP:] GetArchivesClient Success");
            }
            return playersClient;
        }
        /**
         * Get the maximum size of the cover file allowed by the server.
         */
        public ITask<int> GetMaxImageSize()
        {
            //1.When a player saves the game during gameplay, make your game query Huawei game server's restrictions on related archive files.
            return playersClient.LimitThumbnailSize;
        }
        /**
         * Get the maximum size of archive file allowed by the server.
         */
        public ITask<int> GetMaxFileSize()
        {
            return playersClient.LimitDetailsSize;
        }
        /**
         * Commit archive.
         */
        public void Commit(string description, long playedTime, long progress, string ImagePath, String imageType)
        {
            AndroidBitmap testBitmap = new AndroidBitmap(AndroidBitmapFactory.DecodeFile(ImagePath));
            //3- Write the archive metadata(such as the archive description, progress, and cover image) to the ArchiveSummaryUpdate object.
            ArchiveSummaryUpdate archiveSummaryUpdate = new ArchiveSummaryUpdate.Builder().SetActiveTime(playedTime)
                .SetCurrentProgress(progress)
                .SetDescInfo(description)
                .SetThumbnail(testBitmap)
                .SetThumbnailMimeType(imageType)
                .Build();
            //2- Call the ArchiveDetails.set method to write the current archive file of the player to the ArchiveDetails object.
            ArchiveDetails archiveContents = new ArchiveDetails.Builder().Build();
            byte[] arrayOfByte1 = Encoding.UTF8.GetBytes(progress + description + playedTime);
            archiveContents.Set(arrayOfByte1);
            //4- Call the ArchivesClient.addArchive method to submit the archive.
            bool isSupportCache = true;
            ITask<ArchiveSummary> addArchiveTask = Games.GetArchiveClient().AddArchive(archiveContents, archiveSummaryUpdate, isSupportCache);
            ArchiveSummary archiveSummary = null;
            addArchiveTask.AddOnSuccessListener((result) =>
            {
                archiveSummary = result;
                if (archiveSummary != null)
                {
                    Debug.Log("[HMSP:] AddArchive archiveSummary " + archiveSummary.FileName);
                }
            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMS:] AddArchive fail: " + exception.ErrorCode + " :: " + exception.WrappedExceptionMessage + " ::  " + exception.WrappedCauseMessage);
            });
            return;
        }
        // Displaying the Saved Game List
        public void ShowArchive()
        {
            bool param = true;
            ITask<IList<ArchiveSummary>> taskDisplay = playersClient.GetArchiveSummaryList(param);
            taskDisplay.AddOnSuccessListener((result) =>
            {
                if (result != null)
                {
                    Debug.Log("[HMS:]Archive Summary is null ");
                    return;
                }
                if (result.Count > 0)
                    Debug.Log("[HMS:]Archive Summary List size " + result.Count);
            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMS:]Archive Summary Failure " + exception.WrappedExceptionMessage);
            });

            // 1- Displaying the Saved Game List Page of HUAWEI AppAssistant
            string title = "";
            bool allowAddBtn = true, allowDeleteBtn = true;
            int maxArchive = 100;

            SelectedAction += ArchiveSelected;
            AddAction += ArchiveAdded;
            playersClient.ShowArchiveListIntent(title, allowAddBtn, allowDeleteBtn, maxArchive, SelectedAction, AddAction);

            return;
        }

        private void ArchiveSelected(ArchiveSummary archiveSummary)
        {
            Debug.Log("[HMS:] Selected Archive Summary " + archiveSummary.DescInfo);
        }

        private void ArchiveAdded(bool addArhive)
        {
            Debug.Log("[HMS:] Added Archive Summary " + addArhive);
        }

        //Resolving Archive Conflicts
        private void HandleDifference(OperationResult operationResult)
        {
            if (operationResult != null)
            {
                Difference archiveDifference = operationResult.Difference;
                Archive openedArchive = archiveDifference.RecentArchive();
                Archive serverArchive = archiveDifference.ServerArchive;
                // Sample use the server archive to solve diffrence
                if (serverArchive == null)
                {
                    return;
                }
                ITask<OperationResult> task = Games.GetArchiveClient().UpdateArchive(serverArchive);
                task.AddOnSuccessListener((result) =>
                {
                    Debug.Log("OperationResult:" + ((operationResult == null) ? "" : operationResult.Different.ToString()));
                    if (operationResult != null && !operationResult.Different)
                    {
                        Archive archive = operationResult.Difference.RecentArchive();
                        if (archive != null && archive.Summary != null)
                            Debug.Log("OperationResult:" + archive.Summary.Id);
                        else
                            HandleDifference(operationResult);
                    }
                }).AddOnFailureListener((exception) =>
                {
                    Debug.LogError("[HMS:] OperationResult" + exception.ErrorCode);
                });

            }
        }
        //Updating a Saved Game
        public void UpdateSavedGame(String archiveID, ArchiveSummaryUpdate archiveSummaryUpdate, ArchiveDetails archiveContents)
        {
            String archiveId = archiveID;
            ITask<OperationResult> taskUpdateArchive = Games.GetArchiveClient().UpdateArchive(archiveId, archiveSummaryUpdate, archiveContents);
            taskUpdateArchive.AddOnSuccessListener((archiveDataOrConflict) =>
            {
                Debug.Log("[HMS:] taskUpdateArchive" + archiveDataOrConflict.Difference);
                Debug.Log("isDifference:" + ((archiveDataOrConflict == null) ? "" : archiveDataOrConflict.Difference.ToString()));
            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMS:] taskUpdateArchive failed" + exception.ErrorCode);
            });
        }
        //
        public void LoadingSavedGame(String archiveId)
        {
            //Loading a Saved Game
            int conflictPolicy = getConflictPolicy(); // Specify the conflict resolution policy.
            ITask<OperationResult> taskLoadSavedGame;
            if (conflictPolicy == -1)
            {
                taskLoadSavedGame = Games.GetArchiveClient().LoadArchiveDetails(archiveId);
            }
            else
            {
                taskLoadSavedGame = Games.GetArchiveClient().LoadArchiveDetails(archiveId, conflictPolicy);
            }
            taskLoadSavedGame.AddOnSuccessListener((archiveDataOrConflict) =>
            {
                Debug.Log("[HMS:] taskLoadSavedGame" + archiveDataOrConflict.Difference);
                Debug.Log("isDifference:" + ((archiveDataOrConflict == null) ? "" : archiveDataOrConflict.Difference.ToString()));
            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMS:] taskLoadSavedGame failed" + exception.ErrorCode);
            });
        }
        //Delete  Saved Games
        public void DeleteSavedGames(ArchiveSummary archiveSummary)
        {
            // Deleting a Saved Games
            ITask<String> removeArchiveTask = Games.GetArchiveClient().RemoveArchive(archiveSummary);
            removeArchiveTask.AddOnSuccessListener((result) =>
            {
                String deletedArchiveId = result;
                Debug.Log("[HMS:] deletedArchiveId" + result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMS:] removeArchiveTask" + exception.ErrorCode);
            });
        }
        private int getConflictPolicy()
        {
            return 0;
        }

        //To obtain the archive cover, call ArchiveSummary.hasThumbnail to check whether there is a cover thumbnail.
        //If so, call ArchivesClient.getThumbnail to obtain the thumbnail data.
        public void LoadThumbnail(Archive archive)
        {
            if (archive.Summary.HasThumbnail())
            {
                ITask<AndroidBitmap> coverImageTask = Games.GetArchiveClient().GetThumbnail(archive.Summary.Id);
                coverImageTask.AddOnSuccessListener((result) =>
                {
                    Debug.Log("[HMS:] AndroidBitmap put it UI");


                }).AddOnFailureListener((exception) =>
                {
                    Debug.LogError("[HMS:] AndroidBitmap put it UI failed exception"+ exception.Message);
                });
            }
        }


    }

}
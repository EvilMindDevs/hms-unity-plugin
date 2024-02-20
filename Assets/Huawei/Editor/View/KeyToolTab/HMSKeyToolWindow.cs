using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSKeyToolWindow : EditorWindow
    {
        #region Variables
        private const string KeyToolHelpCommand = "-help";

        // File and password information.
        private string filePath = string.Empty;
        private string password = string.Empty;
        private string aliasPassword = string.Empty;
        private List<string> aliases = new List<string>();
        private int selectedAliasIndex = 0;

        // Drag-and-drop state.
        private bool isKeystoreDropped = false;

        // SHA-256 result and command execution.
        private string sha256 = string.Empty;
        private bool isExecutable = false;

        #endregion

        void OnGUI()
        {
            GUILayout.Label("Key Tool Drag & Drop Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Drag and drop your keystore file below to generate SHA-256.", MessageType.Info);
            EditorGUILayout.Space();

            // Drag and drop area.
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drag & Drop Keystore File Here");
            HandleDragAndDrop(dropArea);

            if (isKeystoreDropped)
            {
                ShowPasswordField("Keystore", ref password, ref isExecutable);
                if (aliases.Count > 0)
                {
                    ShowAliasSelection();
                    ShowPasswordField("Alias", ref aliasPassword, ref isExecutable);
                }
                ShowActionButton();
                ShowSHA256Result();
            }
        }
        private void ShowPasswordField(string label, ref string passwordField, ref bool controlState)
        {
            EditorGUILayout.Space();
            string enteredPassword = EditorGUILayout.PasswordField($"{label} Password", passwordField);

            if (!string.IsNullOrWhiteSpace(enteredPassword))
            {
                passwordField = enteredPassword;
                controlState = true;
            }
        }
        private void ShowAliasSelection()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Select your alias:");
            EditorGUI.BeginChangeCheck();
            selectedAliasIndex = EditorGUILayout.Popup("Alias", selectedAliasIndex, aliases.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                sha256 = string.Empty;
                aliasPassword = string.Empty;
            }
        }
        private void ShowActionButton()
        {
            if (isExecutable)
            {
                EditorGUILayout.Space();
                string buttonName = aliases.Count > 0 ? "Obtain SHA-256" : "Run";
                if (GUILayout.Button(buttonName))
                {
                    ExecuteKeyTool();
                }
            }
        }
        private void ShowSHA256Result()
        {
            if (!string.IsNullOrWhiteSpace(sha256))
            {
                EditorGUILayout.Space();
                GUILayout.Label("Selected File: " + Path.GetFileName(filePath));
                EditorGUILayout.Space();
                GUILayout.Label("Your SHA-256: \n" + sha256, EditorStyles.wordWrappedLabel);
                EditorGUILayout.Space();
                if (GUILayout.Button("Copy SHA-256 to Clipboard"))
                {
                    GUIUtility.systemCopyBuffer = sha256;
                    EditorUtility.DisplayDialog("SHA-256 Copied", "The SHA-256 hash has been copied to clipboard.", "OK");
                }
            }
        }
        private void HandleDragAndDrop(Rect dropArea)
        {
            Event evt = Event.current;
            if ((evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform) && dropArea.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (evt.type == EventType.DragPerform || evt.type == EventType.DragUpdated)
                {
                    DragDropOperations();
                }
                else if (evt.type == EventType.DragExited)
                {
                    DragAndDrop.PrepareStartDrag();
                }
                evt.Use();
            }
        }
        private void DragDropOperations()
        {
            DragAndDrop.AcceptDrag();
            DragAndDrop.activeControlID = GUIUtility.GetControlID(FocusType.Passive);

            filePath = DragAndDrop.paths.FirstOrDefault() ?? string.Empty;
            isKeystoreDropped = !string.IsNullOrWhiteSpace(filePath) && filePath.EndsWith(".keystore", StringComparison.OrdinalIgnoreCase);
            if (!isKeystoreDropped)
            {
                EditorUtility.DisplayDialog("Invalid File", "Please drop a .keystore file.", "OK");
            }
            else
            {
                isExecutable = false;
                sha256 = string.Empty;
                aliases = new List<string>();
                password = string.Empty;
                aliasPassword = string.Empty;
            }
        }
        private void ExecuteKeyTool()
        {
            // Check if KeyTool executable is available.
            string keytoolPath = CheckKeyTool();
            if (keytoolPath == "Keytool is not available.")
            {
                EditorUtility.DisplayDialog("Keytool Not Found", "Unable to find Keytool in your environment.", "OK");
                return;
            }

            string arguments = $"-list -v -keystore \"{filePath}\" -storepass {password}";
            if (aliases.Count > 0)
            {
                arguments += $" -keypass {aliasPassword} -alias {aliases[selectedAliasIndex]}";
            }

            // Start the KeyTool process.
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = keytoolPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string errorOutput = process.StandardError.ReadToEnd();
                var errorCheck = errorOutput.ToLower().Contains("error") || output.ToLower().Contains("error");
                process.WaitForExit();

                if (errorCheck)
                {
                    string errorMessage = !string.IsNullOrWhiteSpace(errorOutput) ? errorOutput : output;

                    EditorUtility.DisplayDialog("Keytool Error", $"An error occurred: {errorMessage}", "OK");
                }
                else
                {
                    if (aliases.Count > 0)
                    {
                        sha256 = ExtractSHA256(output);
                    }
                    else
                    {
                        aliases = GetAliasNames(output);
                        if (aliases.Count == 0)
                            EditorUtility.DisplayDialog("No Aliases Found", "No aliases found in the keystore file.", "OK");
                    }
                }
            }
        }
        private string ExtractSHA256(string input)
        {
            var match = Regex.Match(input, @"SHA-?256:\s*(\S+)");
            return match.Success ? match.Groups[1].Value : null;
        }
        private List<string> GetAliasNames(string text)
        {
            return Regex.Matches(text, @"Alias name: (.+)")
                        .Cast<Match>()
                        .Select(m => m.Groups[1].Value.Trim())
                        .ToList();
        }
        private string CheckKeyTool()
        {
            string keytoolPath = "keytool";
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = keytoolPath,
                    Arguments = KeyToolHelpCommand,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

                return keytoolPath;
            }
            catch (Exception)
            {
                //if windows
                return Path.GetDirectoryName(Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines\\AndroidPlayer\\OpenJDK\\bin\\keytool.exe\\")).Replace("\\", "/");
            }
        }
    }
}

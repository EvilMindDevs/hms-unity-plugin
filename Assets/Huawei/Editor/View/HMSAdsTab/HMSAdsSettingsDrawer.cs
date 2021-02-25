using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    internal class HMSAdsSettingsDrawer : VerticalSequenceDrawer
    {
        private TextField.TextField _bannerIdTextField;
        private TextField.TextField _interstitialIdTextField;
        private TextField.TextField _rewardedIdIdTextField;
        private Toggle _testAdstoggle;


        public HMSAdsSettingsDrawer()
        {
            _bannerIdTextField = new TextField.TextField("Banner Ad ID", "", OnBannerIDValueChanged);
            _interstitialIdTextField = new TextField.TextField("Interstitial Ad ID", "", OnInterstitialIDValueChanged);
            _rewardedIdIdTextField = new TextField.TextField("Rewarded Ad ID", "", OnRewardedIDValueChanged);
            _testAdstoggle = new Toggle("Use Test Ads*", false, OnTestAdsToggleChanged);
            _testAdstoggle.SetTooltip("This will overwrite all ads with test ads.");
            SetupSequence();
        }

        private void OnTestAdsToggleChanged(bool obj)
        {

        }

        private void OnRewardedIDValueChanged(string obj)
        {

        }

        private void OnInterstitialIDValueChanged(string obj)
        {

        }

        private void OnBannerIDValueChanged(string obj)
        {

        }

        private void SetupSequence()
        {
            AddDrawer(new HorizontalLine());
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Label.Label("Advertisement ID").SetBold(true), new Spacer()));
            AddDrawer(new Space(10));
            AddDrawer(_bannerIdTextField);
            AddDrawer(new Space(3));
            AddDrawer(_interstitialIdTextField);
            AddDrawer(new Space(3));
            AddDrawer(_rewardedIdIdTextField);
            AddDrawer(new HorizontalLine());

            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Label.Label("Testing").SetBold(true), new Spacer()));
            AddDrawer(new Space(10));
            AddDrawer(_testAdstoggle);
            AddDrawer(new HorizontalLine());
        }
    }
}

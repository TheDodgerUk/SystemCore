using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NetworkSpeedTest
{

    public class SpeedTestRoot : MonoBehaviour
    {

        public void Initialise()
        {
            Core.Environment.OnEnvironmentLoadingComplete += OnEnvironmentLoadingComplete;
        }

        private void OnEnvironmentLoadingComplete()
        {
            Core.SceneLoader.SetMouseLockVisible(false, true);

            string filePathFixed = Application.persistentDataPath.SanitiseSlashes();
            filePathFixed += "/streamingassets";
            Debug.LogError($"filePathFixed {filePathFixed}");
            Utils.IO.CleanFolder(filePathFixed);
            DownloadItems();
        }

        private void DownloadItems()
        {
            Core.GenericLoadingRef.SetActive(true);
            Debug.Log("HouseBuilderRoot UpdateLocalAssetsFromServer");
            Core.Assets.UpdateLocalAssetsFromServer(Core.GenericLoadingRef.OnMessage, Core.GenericLoadingRef.OnProgress, FinishedLoading);
        }

        private void FinishedLoading(float time)
        {
            Core.AssetBundlesRef.CatalogueThumbnailAssetBundleRef.GetLocalVersionNumber((number) =>
            {
                Debug.LogError(number);
            });

            var cat = Core.Catalogue.GetCatalogue;
            Core.Assets.DownloadAndOverwriteSingleAsset(cat[0], null, () =>
            {
                Core.AssetBundlesRef.CatalogueThumbnailAssetBundleRef.DownloadAndOverwrite(null, null);
            });


            //Core.GenericLoadingRef.SetActive(false);

            //////Core.GenericLoadingRef.SetActive(false);
            //////Core.GenericMessageRef.DisplayError("Finished", 5, null);
            /////
            //var fff = Core.Environment.CurrentEnvironment;
            //var cat = Core.Catalogue.GetCatalogue;
            //foreach(var catItem in cat)
            //{

            //    Core.Network.GetAssetVersionNumber(catItem, (number) =>
            //    {
            //        Debug.LogError($"Network, {catItem.ShortName}  {number}");
            //    });

            //    Core.Catalogue.GetLocalAssetVersion(catItem, (number) =>
            //    {
            //        Debug.LogError($"Catalogue, {catItem.ShortName}  {number}");
            //    });

            //    Core.Scene.SpawnObject(catItem, (item) =>
            //    {
            //        Debug.LogError($"MetaData: {catItem.MetaData.Count}     children :{item.transform.GetDirectChildren().Count}");

            //        Core.GenericMessageRef.DisplayMessage("MetaData:", $"MetaData: {catItem.MetaData.Count}     children :{item.transform.GetDirectChildren().Count}", null);

            //        Debug.LogError($"Created object {catItem.ShortName}  valid {null != item} ");
            //        if (null != item)
            //        {
            //            var filters = item.GetComponentsInChildren<MeshFilter>();
            //            Debug.LogError($"There are  {filters.Length}  MeshFilter ");
            //            foreach (var dd in filters)
            //            {
            //                Debug.LogError($"{dd.gameObject.name}  filter is valid :  {dd.mesh != null}");
            //            }
            //        }
            //    });
            //}


        }

    }
}

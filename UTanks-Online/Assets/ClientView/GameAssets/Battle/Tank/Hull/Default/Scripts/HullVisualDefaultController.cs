using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Model;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.Important.TPhysics;
using SecuredSpace.Important.Raven;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTanksClient;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Types;
using UTanksClient.Extensions;
using UTanksClient.Services;

namespace SecuredSpace.Battle.Tank.Hull
{
    public class HullVisualDefaultController : IHullVisualController
    {
        public override void BuildVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard)
        {
            HullVisualDefaultController.BuildVisualShared<HullVisualDefaultController>(visualizableEquipment, eraItemCard, this);
        }

        public override void BuildPreviewVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard)
        {
            if (eraItemCard.ItemData.ContainsKey("PreviewMaterial"))
            {
                eraItemCard.ItemData.Remove("Material");
                eraItemCard.ItemData["Material"] = eraItemCard.ItemData["PreviewMaterial"];
            }
            this.HullVisibleModel = this.gameObject;
            HullVisualDefaultController.BuildVisualShared<HullVisualDefaultController>(visualizableEquipment, eraItemCard, this);
        }

        public static new T BuildVisualShared<T>(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard, T tankVisualController) where T : ITankVisualController
        {
#if AggressiveLog
            List<(string, object)> variableLogger = new List<(string, object)>();
            try
#endif
            {
                var selectedEquip = visualizableEquipment;
                var hullVContr = tankVisualController as IHullVisualController;

                //var materials = ResourcesService.instance.GameAssets.GetDirectory("garage\\skin").GetChildFSObject("card").GetContent<ItemCard>();

                hullVContr.HullResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Hulls[0].PathName + "\\" + selectedEquip.Hulls[0].Grade.ToString()).FillChildContentToItem();

                var globalResources = ResourcesService.instance.GameAssets.GetDirectory("garage\\hull").GetChildFSObject("card").GetContent<ItemCard>();
                hullVContr.HullResources = hullVContr.HullResources + globalResources;

                hullVContr.ColormapResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Colormaps[0].PathName).FillChildContentToItem();


                hullVContr.skinConfig = ConstantService.instance.GetByConfigPath(selectedEquip.Hulls[0].Skins[0].SkinPathName);
                hullVContr.colormapSkinConfig = ConstantService.instance.GetByConfigPath(selectedEquip.Colormaps[0].PathName);
                if (!hullVContr.skinConfig.Deserialized["skinType"]["singleForAllModificationsSkin"].ToObject<bool>())
                {
                    hullVContr.HullSkinResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Hulls[0].Skins[0].SkinPathName + "\\" + selectedEquip.Hulls[0].Grade.ToString()).FillChildContentToItem();
                }
                else
                {
                    hullVContr.HullSkinResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Hulls[0].Skins[0].SkinPathName).FillChildContentToItem();
                }

                hullVContr.HullSkinResources += eraItemCard;

                #region buildVisualTank

                var hullPrefab = hullVContr.HullSkinResources.GetElement<GameObject>("model");
                hullVContr.HullPrefab = hullPrefab;
                if (hullVContr.HullVisibleModel != null && hullVContr.HullVisibleModel != hullVContr.gameObject)
                    GameObject.Destroy(hullVContr.HullVisibleModel);
                var hullInstance = GameObject.Instantiate(hullPrefab, hullVContr.transform);
                hullInstance.transform.localPosition = Vector3.zero;
                hullInstance.transform.localRotation = Quaternion.identity;
                hullVContr.HullVisibleModel = hullInstance;


                var hullMesh = hullInstance.GetComponent<MeshFilter>()?.sharedMesh;

                if(!hullVContr.Preview)
                {
                    for (int i = 1; i < hullVContr.HullAngleColliderHeader.transform.childCount; i++)
                        Destroy(hullVContr.HullAngleColliderHeader.transform.GetChild(i).gameObject);
                    for (int i = 1; i < hullVContr.HullBalanceCollider.transform.childCount; i++)
                        Destroy(hullVContr.HullBalanceCollider.transform.GetChild(i).gameObject);
                    hullVContr.HullBalanceCollider.GetComponents<Collider>().ForEach(x => Destroy(x));
                    hullVContr.TankBoundsObject.GetComponents<Collider>().ForEach(x => Destroy(x));
                    var boundsCollider = hullVContr.gameObject.AddComponent(typeof(BoxCollider)) as BoxCollider;
                    boundsCollider.isTrigger = true;
                    hullVContr.parentTankManager.hullManager.chassisManager.MainBoundsCollider = boundsCollider;

                    hullVContr.TankFrictionCollidersObject.GetComponent<MeshFilter>().mesh = hullMesh;
                    hullVContr.TankFrictionCollidersObject.GetComponent<MeshCollider>().sharedMesh = hullMesh;
                    hullVContr.TankBoundsObject.GetComponent<MeshFilter>().mesh = hullMesh;
                    var boundBox = hullVContr.TankBoundsObject.AddComponent<BoxCollider>();
                    boundBox.size = hullMesh.bounds.size;
                    boundBox.center = hullMesh.bounds.center;

                    boundBox.size = new Vector3(boundBox.size.x, boundBox.size.y / 2, boundBox.size.z);
                    var balanceBox = hullVContr.HullBalanceCollider.AddComponent<BoxCollider>();

                    balanceBox.size = new Vector3(
                        hullVContr.gameplayConfig.GetObject<string>($"balanceColliderConfig\\size\\x").FastFloat(), hullVContr.gameplayConfig.GetObject<string>($"balanceColliderConfig\\size\\y").FastFloat(), hullVContr.gameplayConfig.GetObject<string>($"balanceColliderConfig\\size\\z").FastFloat());

                    balanceBox.center = new Vector3(
                        hullVContr.gameplayConfig.GetObject<string>($"balanceColliderConfig\\center\\x").FastFloat(),
                        hullVContr.gameplayConfig.GetObject<string>($"balanceColliderConfig\\center\\y").FastFloat(),
                        hullVContr.gameplayConfig.GetObject<string>($"balanceColliderConfig\\center\\z").FastFloat());
                }

                hullVContr.normalMaterials.Add(MainHullMaterial, Instantiate(hullVContr.HullSkinResources.GetElement<Material>("Material")));
                hullVContr.normalMaterials.Add(MainTrackMaterial, Instantiate(hullVContr.HullSkinResources.GetElement<Material>("Material")));

                hullVContr.TrackMaterial = hullVContr.normalMaterials[MainTrackMaterial];

                hullVContr.transparentMaterials.Add(MainHullTransparentMaterial, Instantiate(hullVContr.HullSkinResources.GetElement<Material>("TransparentMaterial")));
                hullVContr.transparentMaterials.Add(MainTrackTransparentMaterial, Instantiate(hullVContr.HullSkinResources.GetElement<Material>("TransparentMaterial")));

                hullVContr.normalMaterials.Values.ForEach((Material material) =>
                {
                    //material.SetTexture("_Colormap", ColormapResources.GetElement<Texture2D>("image"));
                    material.SetTexture("_Details", hullVContr.HullSkinResources.GetElement<Texture2D>("details"));
                    material.SetTexture("_Lightmap", hullVContr.HullSkinResources.GetElement<Texture2D>("lightmap"));
                });
                hullVContr.transparentMaterials.Values.ForEach((Material material) =>
                {
                    //material.SetTexture("_Colormap", ColormapResources.GetElement<Texture2D>("image"));
                    material.SetTexture("_Details", hullVContr.HullSkinResources.GetElement<Texture2D>("details"));
                    material.SetTexture("_Lightmap", hullVContr.HullSkinResources.GetElement<Texture2D>("lightmap"));
                });
                
                hullVContr.HullVisibleModel.GetOrAddComponent<MeshRenderer>().materials = hullVContr.normalMaterials.Values.ToArray();

                hullVContr.HullVisibleModel.GetOrAddComponent<ColormapScript>().Setup(hullVContr.colormapSkinConfig, hullVContr.ColormapResources, true);

                for (int i = 0; i < hullInstance.transform.childCount; i++)
                {
                    if (hullInstance.transform.GetChild(i).name.Contains("mount"))
                    {
                        hullVContr.localMountPoint = hullInstance.transform.GetChild(i).transform.localPosition;
                    }
                }
                var allBones = hullInstance.GetComponentsInChildren<Transform>();
                hullVContr.TrackBones = allBones.Where(t => t.name.ToLower().Contains("track")).ToList();
                hullVContr.WheelBones = allBones.Where(t => t.name.ToLower().Contains("wheel")).ToList();

                hullVContr.TrackBoneBasePositions = hullVContr.TrackBones.ToDictionary(b => b, b => b.localPosition);


                hullVContr.TrackBoneBasePositions = hullVContr.TrackBones.ToDictionary(b => b, b => b.localPosition);



                #endregion
            }
#if AggressiveLog
            catch (Exception ex)
            {
                string variableChecker = "";
                variableLogger.ForEach(x => variableChecker += x.Item1 + " = " + x.Item2 + "\n");
                ULogger.Error("RebuildTank\n" + ex.Message + "\n" + ex.StackTrace + "\n" + variableChecker);
            }
#endif
            return (T)tankVisualController;
        }
        protected override void OnRemoveController()
        {
            HullVisualDefaultController.OnRemoveControllerShared(this);
        }

        public override void SetupColormap(ItemCard colormapResource)
        {
            HullVisualDefaultController.SetupColormapShared(this, colormapResource);
        }

        public static new void SetupColormapShared(IHullVisualController hullVisualController, ItemCard colormapResource)
        {
            if (colormapResource == hullVisualController.DeadColormapResources)
            {
                var hullMaterials = hullVisualController.HullVisibleModel.GetComponent<MeshRenderer>().materials;
                hullMaterials.ForEach((Material material) => {
                    material.SetTexture("_Colormap", colormapResource.GetElement<Texture2D>("image"));
                    //material.SetFloat("_TileSize", hullVisualController.normalMaterials[MainHullMaterial].GetFloat("_TileSize"));
                    //material.SetFloat("_Size", hullVisualController.normalMaterials[MainHullMaterial].GetFloat("_Size"));
                    material.SetTextureScale("_Colormap", hullVisualController.normalMaterials[MainHullMaterial].GetTextureScale("_Colormap"));
                });
                hullVisualController.HullVisibleModel.GetComponent<ColormapScript>().DisableColormap();
                return;
            }

            hullVisualController.HullVisibleModel.GetComponent<ColormapScript>().Setup(hullVisualController.colormapSkinConfig, colormapResource);
        }

        public static new void OnRemoveControllerShared(IHullVisualController hullVisualController)
        {

        }

        public override void SetGhostMode(bool enabled)
        {
            HullVisualDefaultController.SetGhostModeShared(this, enabled);
        }

        public static new void SetGhostModeShared(IHullVisualController hullVisualController, bool enable)
        {
            hullVisualController.HullVisibleModel.GetComponent<MeshRenderer>().materials.ForEach(x =>
            {
                if (enable)
                {
                    x.shader = hullVisualController.HullSkinResources.GetElement<Shader>("TransparentShader");
                    x.SetFloat("_Opacity", 1f - (Convert.ToInt32(enable) * 0.5f));
                }
                else
                {
                    x.shader = hullVisualController.HullSkinResources.GetElement<Shader>("Shader");
                }
            });
            hullVisualController.HullVisibleModel.GetComponent<ColormapScript>().Setup(hullVisualController.colormapSkinConfig, hullVisualController.ColormapResources);
        }

        public override void SetTemperature(float temperature)
        {
            HullVisualDefaultController.SetTemperatureShared(this, temperature);
        }

        public static new void SetTemperatureShared(IHullVisualController hullVisualController, float temperature)
        {
            hullVisualController.normalMaterials.ForEach(x =>
            {
                x.Value.SetFloat("_Temperature", temperature);
            });
        }

        public override void MoveAnimation(float MoveMomentX, float MoveMomentY)
        {
            HullVisualDefaultController.MoveAnimationShared(this, MoveMomentX, MoveMomentY, 1);
        }

        public static new void MoveAnimationShared(IHullVisualController hullVisualController, float MoveMomentX, float MoveMomentY, int trackgroup)
        {
            var playedAudio = hullVisualController.hullAudio.audioManager.GetNowPlayingAudioName();
            if (MoveMomentX == 0)
            {
                if (playedAudio.Length == 0 || (playedAudio.Length > 0 && playedAudio[0].IndexOf("idle") == -1))
                {
                    hullVisualController.hullAudio.audioManager.Fade("audio_move");
                    hullVisualController.hullAudio.audioManager.Fade("audio_move_start");
                    hullVisualController.hullAudio.audioManager.Stop("audio_engineidle");
                    hullVisualController.hullAudio.audioManager.Stop("audio_engineidle_loop");
                    hullVisualController.hullAudio.audioManager.PlayBlock(new List<string> { "audio_engineidle", "audio_engineidle_loop" });
                }
            }
            else
            {
                if (playedAudio.Length == 0 || (playedAudio.Length > 0 && playedAudio[0].IndexOf("move") == -1))
                {
                    hullVisualController.hullAudio.audioManager.Fade("audio_engineidle");
                    hullVisualController.hullAudio.audioManager.Fade("audio_engineidle_loop");
                    hullVisualController.hullAudio.audioManager.Stop("audio_move");
                    hullVisualController.hullAudio.audioManager.Stop("audio_move_start");
                    hullVisualController.hullAudio.audioManager.PlayBlock(new List<string> { "audio_move_start", "audio_move" });
                }
            }
        }
    }
}
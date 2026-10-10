using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
#if Steam_Audio
using SteamAudio;
#endif

public class SplTextureMap : MonoBehaviour
{
#if Steam_Audio
    private SteamAudioSource m_SteamAudioSource;
#endif
    private const float OFFSET_FOR_HEAT_MAP = 0.2f;
    private const float ERROR_AMOUNT = -100f;


    public class SplTextureData
    {
#if Steam_Audio
        public Dictionary<TextureMapType, float[,]> m_RawTextureData = new Dictionary<TextureMapType, float[,]>();
        public UnityEngine.Vector3[,] m_RawTextureDataDirection = new UnityEngine.Vector3[0, 0];
        public string GeneratedTextureFileNameNetwork(string saveName, int index, TextureMapType textureMapType) => $"{saveName}_{Core.Network.Users.CurrentUser.m_UserName.ToLower()}_{index}_{textureMapType}.data";
        public string GeneratedTextureFileNameLocally(string saveName, int index, TextureMapType textureMapType) => $"{Application.persistentDataPath}/{GeneratedTextureFileNameNetwork(saveName, index, textureMapType)}";

        public int AmountPerMetre = 2;
        public float MaxHeight = 2;

        public UnityEngine.Vector3 TextureSize;
        public int TextureSizeX;
        public int TextureSizeZ;
        public bool Selected;
#endif
    }

    public SplTextureData m_TextureData = new SplTextureData();


#if Steam_Audio
    public void Initilise(SteamAudioSource steamAudioSource)
    {
        m_SteamAudioSource = steamAudioSource;
    }


    public IEnumerator CreateTextureMaps(Interaction heatMapGameObject, int numberPerMetre, float maxHeight, int index, UnityEngine.Vector3 min, UnityEngine.Vector3 max, Action<string> eta, Action<float> percentage, TaskAction compleated)
    {
        var timer = Stopwatch.RealTime();
        yield return null;
        m_SteamAudioSource.audioEngineSource.GetParameters(m_SteamAudioSource);

        var requiresScene = (m_SteamAudioSource.occlusionMode != OcclusionMode.NoOcclusion || m_SteamAudioSource.reflections);
        var sceneExported = (m_SteamAudioSource.managerData.gameEngineState.Scene().GetScene() != IntPtr.Zero);
        if (requiresScene && !sceneExported)
        {
            Debug.LogError("Scene not found. Make sure to pre-export the scene.");
            yield break;
        }

        var environment = m_SteamAudioSource.managerData.gameEngineState.Environment().GetEnvironment();
        if (environment == IntPtr.Zero)
        {
            compleated.Increment();
            Debug.LogError("environment == IntPtr.Zero");
            yield break;
        }

        var listener = GameObject.FindObjectOfType<AudioListener>();
        if (listener == null)
        {
            compleated.Increment();
            Debug.LogError("listener == null");
            yield break;
        }

        SteamAudio.Vector3 listenerPosition = Common.ConvertVector(listener.transform.position);
        SteamAudio.Vector3 listenerAhead = Common.ConvertVector(listener.transform.forward);
        SteamAudio.Vector3 listenerUp = Common.ConvertVector(listener.transform.up);

        var source = new Source();
        source.position = Common.ConvertVector(transform.position);
        source.ahead = Common.ConvertVector(transform.forward);
        source.up = Common.ConvertVector(transform.up);
        source.right = Common.ConvertVector(transform.right);
        source.directivity = new Directivity();
        source.directivity.dipoleWeight = m_SteamAudioSource.dipoleWeight;
        source.directivity.dipolePower = m_SteamAudioSource.dipolePower;
        source.directivity.callback = IntPtr.Zero;
        source.distanceAttenuationModel = m_SteamAudioSource.distanceAttenuationModel;
        source.airAbsorptionModel = m_SteamAudioSource.airAbsorptionModel;


        int AMOUNT_X = (int)(max.x - min.x);
        int AMOUNT_Z = (int)(max.z - min.z);

        int AmountPerMetre = numberPerMetre;
        UnityEngine.Vector3 TextureSize = max - min;
        float TextureSizeX = AMOUNT_X;
        float TextureSizeZ = AMOUNT_Z;

        int MAX_X = AMOUNT_X * AmountPerMetre;
        int MAX_Z = AMOUNT_Z * AmountPerMetre;

        m_TextureData.m_RawTextureData.Clear();
        m_TextureData.m_RawTextureDataDirection = null;

        if (index == 0 && heatMapGameObject != null)
        {
            Texture2D newImage = new Texture2D(MAX_X, MAX_Z, TextureFormat.ARGB32, false);
            heatMapGameObject.SteamSoundRef.MetaDataRef.m_SplMeshRenderer.material.mainTexture = newImage;
        }

        foreach (TextureMapType item in Enum.GetValues(typeof(TextureMapType)))
        {
            m_TextureData.m_RawTextureData.Add(item, new float[MAX_X, MAX_Z]);
            for (int x = 0; x < MAX_X; x++)
            {
                for (int z = 0; z < MAX_Z; z++)
                {
                    m_TextureData.m_RawTextureData[item][x, z] = ERROR_AMOUNT;
                }
            }
        }

        m_TextureData.m_RawTextureDataDirection = new UnityEngine.Vector3[MAX_X, MAX_Z];
        Debug.Log($"Total number of reads: {MAX_X * MAX_Z}");

        DateTime startTime = DateTime.Now;
        UnityEngine.Vector3 testingPosition = UnityEngine.Vector3.zero;

        EstimateTimeLeftForLoops amountLeft = new EstimateTimeLeftForLoops(MAX_X);
        int totalLoopCount = MAX_X * MAX_Z;
        int loopCountToNull = totalLoopCount / 10;
        int currentLoopCount = 0;

        for (int x = 0; x < MAX_X; x++)
        {
            float perentageAmount = (float)x / (float)(MAX_X);
            percentage?.Invoke(perentageAmount);

            float offsetX = (float)x / (float)AmountPerMetre;
            string timeRemaining = amountLeft.RemainingTimeReadable(x);
            eta?.Invoke(timeRemaining);

            ///////yield return null; // this slows it down 
            for (int z = 0; z < MAX_Z; z++)
            {
                ///////yield return null; // this slows it down 
                currentLoopCount++;
                if (currentLoopCount % loopCountToNull == 0)
                {
                    // using it herefor the null so you can stop the corutine
                    // with the other nulls, it was taking 45 seconds now take 2 seconds
                    // with null the system does a compplete update loop
                    // might have to offset the other ones, and 
                    // make the loopCountToNull more depening if more sources
                    yield return null;
                }

                float offsetZ = (float)z / (float)AmountPerMetre;

                float testForY = OFFSET_FOR_HEAT_MAP;
                UnityEngine.Vector3 rayPosition = new UnityEngine.Vector3(min.x + offsetX, maxHeight, min.z + offsetZ);
                var allHit = Physics.RaycastAll(rayPosition, UnityEngine.Vector3.down, Mathf.Infinity, Layers.NavigationMask).ToList();


                int foundIndex = -1;

                if (allHit.Count > 0)
                {
                    testForY = allHit[0].point.y + OFFSET_FOR_HEAT_MAP;
                    if (heatMapGameObject != null)
                    {
                        foundIndex = allHit.FindIndex(e => e.collider.gameObject == heatMapGameObject.SteamSoundRef.MetaDataRef.m_SplGameObject); // heatMapGameObject layer been set to Layers.Navigation
                        if (foundIndex >= 0)
                        {
                            testForY = allHit[foundIndex].point.y + OFFSET_FOR_HEAT_MAP;
                        }
                    }
                }

                if (testForY > (maxHeight + OFFSET_FOR_HEAT_MAP))
                {
                    Debug.LogError($"testForY {testForY}, maxHeight {maxHeight},   {rayPosition.ToAccurateString()}");
                }

                // move all items to get correct data
                testingPosition = new UnityEngine.Vector3(min.x + offsetX, testForY, min.z + offsetZ);
                listenerPosition = Common.ConvertVector(testingPosition);
                listenerUp = Common.ConvertVector(UnityEngine.Vector3.up);

                try
                {
                    m_SteamAudioSource.directPath = PhononCore.iplGetDirectSoundPath(environment, listenerPosition,
                        listenerAhead, listenerUp, source, m_SteamAudioSource.sourceRadius, m_SteamAudioSource.occlusionSamples,
                        m_SteamAudioSource.occlusionMode, m_SteamAudioSource.occlusionMethod);
                }
                catch (Exception e)
                {
                    Debug.Log($"System failed here, this will allow it to pass Error : {e.Message}");
                }
                int textureX = x;
                int textureZ = z;

                if (heatMapGameObject != null)
                {
                    if (allHit.Count > 0)
                    {
                        if (foundIndex >= 0)
                        {
                            textureX = (int)(allHit[foundIndex].textureCoord.x * MAX_X);
                            textureZ = (int)(allHit[foundIndex].textureCoord.y * MAX_Z);
                        }
                        else
                        {
                            Debug.Log($"allHit.Count {allHit.Count}  missed ,x: {x}, z: {z}     real pos X: {min.x + offsetX}, real pos X: {min.z + offsetZ}");
                        }
                    }
                    else
                    {
                        Debug.Log($"allHit.Count {allHit.Count}  missed ,x: {x}, z: {z}      real pos X: {min.x + offsetX}, real pos X: {min.z + offsetZ}");
                        continue;
                    }
                }



                m_TextureData.m_RawTextureData[TextureMapType.occlusionFactor][textureX, textureZ] = m_SteamAudioSource.directPath.occlusionFactor;
                m_TextureData.m_RawTextureData[TextureMapType.propagationDelay][textureX, textureZ] = m_SteamAudioSource.directPath.propagationDelay;
                m_TextureData.m_RawTextureData[TextureMapType.distanceAttenuation][textureX, textureZ] = m_SteamAudioSource.directPath.distanceAttenuation;

                m_TextureData.m_RawTextureData[TextureMapType.transmissionFactorLow][textureX, textureZ] = m_SteamAudioSource.directPath.transmissionFactorLow;
                m_TextureData.m_RawTextureData[TextureMapType.transmissionFactorMid][textureX, textureZ] = m_SteamAudioSource.directPath.transmissionFactorMid;
                m_TextureData.m_RawTextureData[TextureMapType.transmissionFactorHigh][textureX, textureZ] = m_SteamAudioSource.directPath.transmissionFactorHigh;

                m_TextureData.m_RawTextureData[TextureMapType.airAbsorptionLow][textureX, textureZ] = m_SteamAudioSource.directPath.airAbsorptionLow;
                m_TextureData.m_RawTextureData[TextureMapType.airAbsorptionMid][textureX, textureZ] = m_SteamAudioSource.directPath.airAbsorptionMid;
                m_TextureData.m_RawTextureData[TextureMapType.airAbsorptionHigh][textureX, textureZ] = m_SteamAudioSource.directPath.airAbsorptionHigh;

                m_TextureData.m_RawTextureData[TextureMapType.directivityFactor][textureX, textureZ] = m_SteamAudioSource.directPath.directivityFactor;
                m_TextureData.m_RawTextureDataDirection[textureX, textureZ] = Common.ConvertVector(m_SteamAudioSource.directPath.direction);
            }
        }


        m_TextureData.AmountPerMetre = numberPerMetre;
        m_TextureData.TextureSize = max - min;
        m_TextureData.TextureSizeX = AMOUNT_X;
        m_TextureData.TextureSizeZ = AMOUNT_Z;

        // fix the missing data, caused by float point errors

        int missingTotalCount = 0;
        for (int x = 0; x < MAX_X; x++)
        {
            for (int z = 0; z < MAX_Z; z++)
            {
                if (m_TextureData.m_RawTextureData[TextureMapType.occlusionFactor][x, z] < -10)
                {
                    missingTotalCount++;
                }
            }
        }

        int missingCurrentCount = 0;
        amountLeft = new EstimateTimeLeftForLoops(missingTotalCount);

        for (int x = 0; x < MAX_X; x++)
        {
            for (int z = 0; z < MAX_Z; z++)
            {
                if (m_TextureData.m_RawTextureData[TextureMapType.occlusionFactor][x, z] < -10)
                {
                    missingCurrentCount++;

                    if (missingCurrentCount % loopCountToNull == 0)
                    {
                        // using it herefor the null so you can stop the corutine
                        // with the other nulls, it was taking 45 seconds now take 2 seconds
                        // with null the system does a compplete update loop
                        // might have to offset the other ones, and 
                        // make the loopCountToNull more depening if more sources
                        yield return null;
                    }

                    float perentageAmount = (float)missingCurrentCount / (float)(missingTotalCount);
                    percentage?.Invoke(perentageAmount);

                    foreach (TextureMapType item in Enum.GetValues(typeof(TextureMapType)))
                    {
                        ValidateBadData(m_TextureData.m_RawTextureData[item], x, z, MAX_X, MAX_Z);
                    }
                }
            }
        }


        timer.Stop();
        Debug.Log($"Combined Finished making images took: {timer.ToReadable()}, AMOUNT_X {AMOUNT_X}, AMOUNT_Z {AMOUNT_Z}, AMOUNT_PER_METRE {m_TextureData.AmountPerMetre}");
        Debug.Log($"Time taken {timer.ToReadable()}");


        foreach (TextureMapType item in Enum.GetValues(typeof(TextureMapType)))
        {
            SaveAudioTextureMapData(index, MAX_X, MAX_Z, m_TextureData.m_RawTextureData[item], item);
        }


        compleated.Increment();
    }

    private void ValidateBadData(float[,] data, int x, int z, int maxX, int maxZ)
    {
        float left = data[x.ClampDecrement(maxX - 1), z];
        float right = data[x.ClampIncrement(maxX - 1), z];
        float forward = data[x, z.ClampIncrement(maxZ - 1)];
        float back = data[x, z.ClampDecrement(maxZ - 1)];

        int count = 0;
        float amount = 0;
        if (((left < (ERROR_AMOUNT + 10)) == false) && (left != float.NaN))
        {
            amount += left;
            count++;
        }

        if (((right < (ERROR_AMOUNT + 10)) == false) && (right != float.NaN))
        {
            amount += right;
            count++;
        }

        if (((forward < (ERROR_AMOUNT + 10)) == false) && (forward != float.NaN))
        {
            amount += forward;
            count++;
        }

        if (((back < (ERROR_AMOUNT + 10)) == false) && (back != float.NaN))
        {
            amount += back;
            count++;
        }

        if (count != 0)
        {
            data[x, z] = amount / count;
        }
    }

    private void SaveAudioTextureMapData(int index, int sizeX, int sizeZ, float[,] data, TextureMapType textureMapType)
    {
        string filePath = m_TextureData.GeneratedTextureFileNameLocally(HouseBuilder.HBGUIManager.Instance.LoadSaveMenuManagerRef.CurrentFileName, index, textureMapType);
        if (Directory.Exists(Path.GetDirectoryName(filePath)) == false)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }
        Json.JsonNet.WriteToFile<float[,]>(data, filePath, true);
    }


    public void LoadAudioTextureMapData(int index)
    {
        m_TextureData.m_RawTextureData.Clear();
        foreach (TextureMapType item in Enum.GetValues(typeof(TextureMapType)))
        {
            m_TextureData.m_RawTextureData.Add(item, new float[m_TextureData.TextureSizeX * m_TextureData.AmountPerMetre, m_TextureData.TextureSizeZ * m_TextureData.AmountPerMetre]);
        }

        foreach (TextureMapType item in Enum.GetValues(typeof(TextureMapType)))
        {
            m_TextureData.m_RawTextureData[item] = LoadAudioTextureMapData(index, m_TextureData.TextureSizeX * m_TextureData.AmountPerMetre, m_TextureData.TextureSizeZ * m_TextureData.AmountPerMetre, item);
        }
    }

    private float[,] LoadAudioTextureMapData(int index, int sizeX, int sizeZ, TextureMapType textureMapType)
    {
        string filePath = "";
        if (HouseBuilder.HouseBuilderRoot.Instance != null)
        {
            filePath = m_TextureData.GeneratedTextureFileNameLocally(HouseBuilder.HBGUIManager.Instance.LoadSaveMenuManagerRef.CurrentFileName, index, textureMapType);
        }
        else
        {
            filePath = m_TextureData.GeneratedTextureFileNameLocally(HouseViewer.HouseViewerRoot.Instance.CurrentFileName, index, textureMapType);
        }
        if (Directory.Exists(Path.GetDirectoryName(filePath)) == true)
        {
            if (Json.FileExits(filePath) == true)
            {
                return Json.JsonNet.ReadFromFile<float[,]>(filePath);
            }
        }
        return null;
    }
#endif
}

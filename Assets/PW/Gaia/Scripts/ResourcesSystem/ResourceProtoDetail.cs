using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Gaia.GaiaConstants;
#if FLORA_PRESENT
using ProceduralWorlds.Flora;
using static ProceduralWorlds.Flora.FloraCommonData;
#endif
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    /// <summary>
    /// Used to serialise the detail prototypes
    /// </summary>
    [System.Serializable]
    public class ResourceProtoDetail
    {

        [Tooltip("Resource name.")]
        public string m_name;
        [Tooltip("Render mode.")]
        public DetailRenderMode m_renderMode = DetailRenderMode.Grass;
        [Tooltip("Detail prototype - used by vertex lit render mode.")]
        public GameObject m_detailProtoype;
        [HideInInspector]
        public string m_detailPrototypeFileName; // Used for re-association
        [Tooltip("The texture that represents the grass and used by grass and billboard grass render mode.")]
        public Texture2D m_detailTexture;
        [HideInInspector]
        public string m_detailTextureFileName; // Used for re-association
        [Tooltip("Minimum width. Lower limit of the width of the clumps of grass that are generated.")]
        public float m_minWidth = 1f;
        [Tooltip("Maximum width. Upper limit of the width of the clumps of grass that are generated.")]
        public float m_maxWidth = 2f;
        [Tooltip("Minimum height. Lower limit of the height of the clumps of grass that are generated.")]
        public float m_minHeight = 1f;
        [Tooltip("Maximum height. Upper limit of the height of the clumps of grass that are generated.")]
        public float m_maxHeight = 2f;
        [Tooltip("Controls the approximate size of the alternating patches, with higher values indicating more variation within a given area."), Range(0f, 1f)]
        public float m_noiseSpread = 0.3f;
        [Tooltip("Healthy grass clump colour.")]
        public Color m_healthyColour = Color.white;
        [Tooltip("Dry grass clump colour.")]
        public Color m_dryColour = Color.white;
        [Tooltip("Use the PW Grass system to replace this terrain detail with a high-quality instanced indirect mesh.")]
        public bool m_usePWGrass = false;

#if FLORA_PRESENT
        public DetailScriptableObject DetailerSettingsObject
        {
            get
            {
                if (m_pwDetailerSettingsObject == null)
                {
                    if (!string.IsNullOrEmpty(m_detailerSettingsObjectAssetGUID))
                    {
#if UNITY_EDITOR
                       string assetPath = AssetDatabase.GUIDToAssetPath(m_detailerSettingsObjectAssetGUID);

                        //This can be multiple objects since the scriptable object can be embedded in a spawner settings file
                        UnityEngine.Object[] allObjects = AssetDatabase.LoadAllAssetsAtPath(assetPath);

                        //Looking for the object via the instance ID
                        foreach (UnityEngine.Object obj in allObjects)
                        {
                            if (obj.GetType() == typeof(DetailScriptableObject) && obj.GetInstanceID() == m_detailerSettingsObjectInstanceID)
                            {
                                //found it, cast it into the correct type to store it
                                m_pwDetailerSettingsObject = (DetailScriptableObject)obj;
                                //is this a file that was embedded within other assets at this path?
                                if (allObjects.Count() > 1)
                                {
                                    //we need to create a copy and save it to not work with the embedded file
                                    m_pwDetailerSettingsObject = ScriptableObject.Instantiate(m_pwDetailerSettingsObject);
                                    SaveGrassSettingsFile();
                                }
                            }
                        }
#endif
                    }
                }
                return m_pwDetailerSettingsObject;
            }
            set
            {
                if (value != m_pwDetailerSettingsObject)
                {
                    m_pwDetailerSettingsObject = value;
                    if (value != null)
                    {
#if UNITY_EDITOR
                        m_detailerSettingsObjectAssetGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
                        m_detailerSettingsObjectInstanceID = value.GetInstanceID();
#endif
                    }
                    else
                    {
                        m_detailerSettingsObjectAssetGUID = "";
                        m_detailerSettingsObjectInstanceID = -1;
                    }
                }
            }
        }
        //This needs to be serialized still to survive recompiles, etc.!
        [SerializeField]
        private DetailScriptableObject m_pwDetailerSettingsObject;

        /// <summary>
        /// Creates an instantiated (separate) copy of the detailer settings object in the Gaia User Data directory.
        /// </summary>
        public void InstantiateDetailerSettingsGO()
        {
            if (m_pwDetailerSettingsObject == null)
            {
                if (!string.IsNullOrEmpty(m_detailerSettingsObjectAssetGUID))
                {
#if UNITY_EDITOR
                    string assetPath = AssetDatabase.GUIDToAssetPath(m_detailerSettingsObjectAssetGUID);

                    //This can be multiple objects since the scriptable object can be embedded in a spawner settings file
                    UnityEngine.Object[] allObjects = AssetDatabase.LoadAllAssetsAtPath(assetPath);

                    //Looking for the object via the instance ID
                    foreach (UnityEngine.Object obj in allObjects)
                    {
                        if (obj.GetType() == typeof(DetailScriptableObject) && obj.GetInstanceID() == m_detailerSettingsObjectInstanceID)
                        {
                            //found it, cast it into the correct type to store it
                            m_pwDetailerSettingsObject = (DetailScriptableObject)obj;
                        }
                    }
#endif
                }
            }
            if (m_pwDetailerSettingsObject != null)
            {
                m_pwDetailerSettingsObject = ScriptableObject.Instantiate(m_pwDetailerSettingsObject);
                SaveGrassSettingsFile();
            }
        }

#endif
        public string m_detailerSettingsObjectAssetGUID;
        public int m_detailerSettingsObjectInstanceID;



        [Tooltip("DNA - Used by the spawner to control how and where the grass will be spawned.")]
        public ResourceProtoDNA m_dna;
        [Tooltip("SPAWN CRITERIA - Spawn criteria are run against the terrain to assess its fitness in a range of 0..1 for use by this resource. If you add multiple criteria then the fittest one will be selected.")]
        public SpawnCritera[] m_spawnCriteria = new SpawnCritera[0];
        [Tooltip("SPAWN EXTENSIONS - Spawn extensions allow fitness, spawning and post spawning extensions to be made to the spawning system.")]
        public SpawnRuleExtension[] m_spawnExtensions = new SpawnRuleExtension[0];
        //[Tooltip("SPAWN MASKS - This list of masks can be used to determine where the terrain detail will appear on the terrain.")]
        //public ImageMask[] m_imageMasks = new ImageMask[0];
        public bool m_dnaFoldedOut;

        /// <summary>
        /// Initialise the detail
        /// </summary>
        /// <param name="spawner">The spawner it belongs to</param>
        public void Initialise(Spawner spawner)
        {
            foreach (SpawnCritera criteria in m_spawnCriteria)
            {
                criteria.Initialise(spawner);
            }
        }

        /// <summary>
        /// Determine whether this has active criteria
        /// </summary>
        /// <returns>True if has actrive criteria</returns>
        public bool HasActiveCriteria()
        {
            for (int idx = 0; idx < m_spawnCriteria.Length; idx++)
            {
                if (m_spawnCriteria[idx].m_isActive)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Set up the asset associations, return true if something changes. Can only be run when the editor is present.
        /// </summary>
        /// <returns>True if something changes</returns>
        public bool SetAssetAssociations()
        {
            bool isModified = false;

#if UNITY_EDITOR
            if (m_detailProtoype != null)
            {
                string fileName = Path.GetFileName(AssetDatabase.GetAssetPath(m_detailProtoype));
                if (fileName != m_detailPrototypeFileName)
                {
                    m_detailPrototypeFileName = fileName;
                    isModified = true;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(m_detailPrototypeFileName))
                {
                    m_detailPrototypeFileName = "";
                    isModified = true;
                }
            }

            if (m_detailTexture != null)
            {
                string fileName = Path.GetFileName(AssetDatabase.GetAssetPath(m_detailTexture));
                if (fileName != m_detailTextureFileName)
                {
                    m_detailTextureFileName = fileName;
                    isModified = true;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(m_detailTextureFileName))
                {
                    m_detailTextureFileName = "";
                    isModified = true;
                }
            }
#endif

            return isModified;
        }


        /// <summary>
        /// Associate any unallocated assets to this resource. Return true if something changes.
        /// </summary>
        /// <returns>True if the prototype was in some way modified</returns>
        public bool AssociateAssets()
        {
            bool isModified = false;

#if UNITY_EDITOR
            if (m_detailProtoype == null)
            {
                if (!string.IsNullOrEmpty(m_detailPrototypeFileName))
                {
                    m_detailProtoype = GaiaUtils.GetAsset(m_detailPrototypeFileName, typeof(UnityEngine.GameObject)) as GameObject;
                    if (m_detailProtoype != null)
                    {
                        isModified = true;
                    }
                }
            }

            if (m_detailTexture == null)
            {
                if (!string.IsNullOrEmpty(m_detailTextureFileName))
                {
                    m_detailTexture = GaiaUtils.GetAsset(m_detailTextureFileName, typeof(UnityEngine.Texture2D)) as Texture2D;
                    if (m_detailTexture != null)
                    {
                        isModified = true;
                    }
                }
            }
#endif

            return isModified;
        }

        /// <summary>
        /// Determine whether this has active criteria that checks textures
        /// </summary>
        /// <returns>True if has active criteria that checks textures</returns>
        public bool ChecksTextures()
        {
            for (int idx = 0; idx < m_spawnCriteria.Length; idx++)
            {
                if (m_spawnCriteria[idx].m_isActive && m_spawnCriteria[idx].m_checkTexture)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determine whether this has active criteria that checks proximity
        /// </summary>
        /// <returns>True if has active criteria that checks proximity</returns>
        public bool ChecksProximity()
        {
            for (int idx = 0; idx < m_spawnCriteria.Length; idx++)
            {
                if (m_spawnCriteria[idx].m_isActive && m_spawnCriteria[idx].m_checkProximity)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add tags to the list if they are not already there
        /// </summary>
        /// <param name="tagList">The list to add the tags to</param>
        public void AddTags(ref List<string> tagList)
        {
            for (int idx = 0; idx < m_spawnCriteria.Length; idx++)
            {
                if (m_spawnCriteria[idx].m_isActive && m_spawnCriteria[idx].m_checkProximity)
                {
                    if (!tagList.Contains(m_spawnCriteria[idx].m_proximityTag))
                    {
                        tagList.Add(m_spawnCriteria[idx].m_proximityTag);
                    }
                }
            }
        }

#if FLORA_PRESENT
        public void SaveGrassSettingsFile(bool allowOverWrite = false, string name ="")
        {
#if UNITY_EDITOR
            string path = GaiaDirectories.GetTerrainDetailsPath();
            if (!allowOverWrite)
            {
                //We create a new settings asset for this resorurce for the first time.
                //Make sure the name is unique, do not want to overwrite another settings file
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                //strip off the extension, we need an unique name BEFORE the extension
                string[] existingFileNames = directoryInfo.GetFiles().Select(x => x.Name.Replace(".asset", "")).ToArray();
                DetailerSettingsObject.m_data.Name = ObjectNames.GetUniqueName(existingFileNames, m_name);
            }
            else
            {
                if (name != "")
                {
                    DetailerSettingsObject.m_data.Name = name;
                }
                else
                {
                    DetailerSettingsObject.m_data.Name = m_name;
                }
            }
            AssetDatabase.CreateAsset(DetailerSettingsObject, path + "/" + DetailerSettingsObject.m_data.Name + ".asset");
            m_detailerSettingsObjectAssetGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(DetailerSettingsObject));
            m_detailerSettingsObjectInstanceID = DetailerSettingsObject.GetInstanceID();
#endif
        }

        public void UpdateAllTerrainsWithNewSettingsSO(DetailScriptableObject oldObject)
        {
            if (GaiaUtils.HasDynamicLoadedTerrains())
            {
                Action<Terrain> act = (terrain) => UpdateSingleTerrainWithNewSettingsSO(terrain, oldObject);
                GaiaUtils.CallFunctionOnDynamicLoadedTerrains(act,true,null,"Updating Detail Settings");
            }
            else
            {
                foreach (Terrain t in Terrain.activeTerrains)
                {
                    UpdateSingleTerrainWithNewSettingsSO(t, oldObject);
                }
            }
        }


        public void UpdateSingleTerrainWithNewSettingsSO(Terrain t, DetailScriptableObject oldObject)
        {
            if (oldObject == null)
            {
                return;
            }

            if (DetailerSettingsObject == null)
            {
                return;
            }

            FloraTerrainTile dtt = t.GetComponent<FloraTerrainTile>();
            if (dtt == null)
            {
                return;
            }
            if (dtt.m_detailObjectList == null)
            {
                return;
            }

            //We match by the detail settings object, this should be precise enough to get the correct entry in the list
            int listIndex = dtt.m_detailObjectList.FindIndex(x => x.DetailScriptableObject == oldObject);

            if (listIndex == -1)
            {
                //not found, exit
                return;
            }

            //DetailOverrideData is a struct, need to overwrite it in the list if it already exists
            DetailOverrideData detailOverrideData = new DetailOverrideData();
            detailOverrideData.DetailScriptableObject = DetailerSettingsObject;
            detailOverrideData.SourceDataType = SourceDataType.Detail;
            detailOverrideData.SourceDataIndex = dtt.m_detailObjectList[listIndex].SourceDataIndex;
            detailOverrideData.DebugColor = dtt.m_detailObjectList[listIndex].DebugColor;
            dtt.m_detailObjectList[listIndex] = detailOverrideData;
        }

        public void RemoveSettingsSOFromAllTerrains()
        {
            if (GaiaUtils.HasDynamicLoadedTerrains())
            {
                Action<Terrain> act = (terrain) => RemoveSettingsSOFromSingleTerrain(terrain);
                GaiaUtils.CallFunctionOnDynamicLoadedTerrains(act, true, null, "Removing Detail Setting");
            }
            else
            {
                foreach (Terrain t in Terrain.activeTerrains)
                {
                    RemoveSettingsSOFromSingleTerrain(t);
                }
            }
        }

        public void RemoveSettingsSOFromSingleTerrain(Terrain t)
        {
#if UNITY_EDITOR
            if (DetailerSettingsObject == null)
            {
                return;
            }

            FloraTerrainTile dtt = t.GetComponent<FloraTerrainTile>();
            if (dtt == null)
            {
                return;
            }
            if (dtt.m_detailObjectList == null)
            {
                return;
            }

            //We match by the detail settings object, this should be precise enough to get the correct entry in the list
            int listIndex = dtt.m_detailObjectList.FindIndex(x => x.DetailScriptableObject == DetailerSettingsObject);

            if (listIndex == -1)
            {
                //not found, exit
                return;
            }
            else
            {
                dtt.m_detailObjectList.RemoveAt(listIndex);
            }

            //if no detail configurations remaining, we can as well strip off the component
            if (dtt.m_detailObjectList.Count == 0)
            {
                Component.DestroyImmediate(dtt);
            }
#endif

        }

        public void CopySettingsAndApply(DetailScriptableObject oldObject)
        {
            string oldName = oldObject.name;
            GaiaUtils.CopyFields(m_pwDetailerSettingsObject, oldObject, true);
            DetailerSettingsObject = oldObject;
            DetailerSettingsObject.name = oldName;
        }

        public void CreateNewDetailerSettingsObject()
        {
            DetailerSettingsObject = ScriptableObject.CreateInstance<DetailScriptableObject>();
            DetailerSettingsObject.m_data = new DetailData();
            SaveGrassSettingsFile(false);
        }
#endif
    }
}
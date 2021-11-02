#if HDPipeline
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Gaia
{
    public enum DensityVolumeResolution { VeryLow, Low, Medium, High, VeryHigh, Ultra, Custom }
    public enum DensityVolumeEffectType { VeryLightHase, LightHase, ModerateHase, HighHase, ExtremeHase, Custom }

    [System.Serializable]
    public class DensityVolumeProfile
    {
        public Color m_singleScatteringAlbedo = Color.white;
        public float m_fogDistance = 250f;
        public Vector3 m_size = new Vector3(20f, 20f, 20f);
        public float m_blendDistance = 0f;
        public bool m_invertBlend = false;
        public float m_distanceFadeStart = 10000f;
        public float m_distanceFadeEnd = 10000f;
        public Texture3D m_texture;
        public Vector3 m_scrollSpeed = Vector3.zero;
        public Vector3 m_tiling = Vector3.one;
        public DensityVolumeResolution m_resolution = DensityVolumeResolution.High;
        public DensityVolumeEffectType m_effectType = DensityVolumeEffectType.LightHase;
    }
    [ExecuteAlways]
    public class HDRPDensityVolumeController : MonoBehaviour
    {
        //Static
        public static HDRPDensityVolumeController Instance
        {
            get { return m_instance; }
            set
            {
                m_instance = value;
            }
        }

        [SerializeField] private static HDRPDensityVolumeController m_instance;

        //Public
        public DensityVolumeProfile DensityVolumeProfile
        {
            get { return m_densityVolumeProfile; }
            set
            {
                m_densityVolumeProfile = value;
            }
        }
        public Camera m_mainCamera;
        //Private
        [SerializeField] private DensityVolumeProfile m_densityVolumeProfile = new DensityVolumeProfile();
        [SerializeField] private DensityVolume m_volume;
        [SerializeField] private Camera m_editorMainCamera;
        private const string DensityVolumeName = "Gaia World Density Volume";

#region Unity Functions

        /// <summary>
        /// Apply on enable
        /// </summary>
        private void OnEnable()
        {
            Initialize();
        }
        /// <summary>
        /// Applies on disable
        /// </summary>
        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdateVolumeController;
#endif
        }
        /// <summary>
        /// Applies on destroy
        /// </summary>
        private void OnDestroy()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdateVolumeController;
#endif
        }
        /// <summary>
        /// Updates every frame
        /// </summary>
        private void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (m_mainCamera != null)
            {
                UpdateVolumeTransform(m_mainCamera.transform.position, Quaternion.identity);
            }
        }

#endregion
#region Utils

        /// <summary>
        /// Applies the density volume changes
        /// </summary>
        public void ApplyChanges()
        {
            m_instance = this;

            if (m_volume == null)
            {
                m_volume = CreateOrGetVolume();
            }

            m_volume.parameters.albedo = DensityVolumeProfile.m_singleScatteringAlbedo;
            m_volume.parameters.meanFreePath = DensityVolumeProfile.m_fogDistance;
            m_volume.parameters.size = GetEffectSizeType(DensityVolumeProfile);
            m_volume.parameters.anisotropy = DensityVolumeProfile.m_blendDistance;
            m_volume.parameters.invertFade = DensityVolumeProfile.m_invertBlend;
            m_volume.parameters.distanceFadeStart = DensityVolumeProfile.m_distanceFadeStart;
            m_volume.parameters.distanceFadeEnd = DensityVolumeProfile.m_distanceFadeEnd;
            m_volume.parameters.volumeMask = DensityVolumeProfile.m_texture;
            m_volume.parameters.textureScrollingSpeed = DensityVolumeProfile.m_scrollSpeed;
            m_volume.parameters.textureTiling = GetResolution(DensityVolumeProfile);
        }

        /// <summary>
        /// Gets the tiling resolution
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        private Vector3 GetResolution(DensityVolumeProfile profile)
        {
            Vector3 resolution = Vector3.one;
            if (profile == null)
            {
                return resolution;
            }

            switch (profile.m_resolution)
            {
                case DensityVolumeResolution.VeryLow:
                {
                    resolution = new Vector3(128f, 128f, 128f);
                    break;
                }
                case DensityVolumeResolution.Low:
                {
                    resolution = new Vector3(256f, 256f, 256f);
                    break;
                }
                case DensityVolumeResolution.Medium:
                {
                    resolution = new Vector3(512f, 512f, 512f);
                    break;
                }
                case DensityVolumeResolution.High:
                {
                    resolution = new Vector3(1024f, 1024f, 1024f);
                    break;
                }
                case DensityVolumeResolution.VeryHigh:
                {
                    resolution = new Vector3(2048f, 2048f, 2048f);
                    break;
                }
                case DensityVolumeResolution.Ultra:
                {
                    resolution = new Vector3(4096f, 4096f, 4096f);
                    break;
                }
                case DensityVolumeResolution.Custom:
                {
                    resolution = profile.m_tiling;
                    break;
                }
            }

            return resolution;
        }
        /// <summary>
        /// Gets the size effect type
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        private Vector3 GetEffectSizeType(DensityVolumeProfile profile)
        {
            Vector3 size = new Vector3(5f, 5f, 5f);
            if (profile == null)
            {
                return size;
            }

            switch (profile.m_effectType)
            {
                case DensityVolumeEffectType.VeryLightHase:
                {
                    size = new Vector3(10f, 10f, 10f);
                    break;
                }
                case DensityVolumeEffectType.LightHase:
                {
                    size = new Vector3(30f, 30f, 30f);
                    break;
                }
                case DensityVolumeEffectType.ModerateHase:
                {
                    size = new Vector3(55f, 55f, 55f);
                    break;
                }
                case DensityVolumeEffectType.HighHase:
                {
                    size = new Vector3(120f, 120f, 120f);
                    break;
                }
                case DensityVolumeEffectType.ExtremeHase:
                {
                    size = new Vector3(1000f, 1000f, 1000f);
                    break;
                }
                case DensityVolumeEffectType.Custom:
                {
                    size = profile.m_size;
                    break;
                }
            }

            return size;
        }
        /// <summary>
        /// Used to Initialize
        /// </summary>
        private void Initialize()
        {
            transform.hideFlags = HideFlags.HideInInspector;
            ApplyChanges();
            m_mainCamera = GaiaUtils.GetCamera();
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorApplication.update -= EditorUpdateVolumeController;
                EditorApplication.update += EditorUpdateVolumeController;
            }
            else
            {
                EditorApplication.update -= EditorUpdateVolumeController;
            }
#endif
        }
        /// <summary>
        /// Gets or creates a density volume
        /// </summary>
        /// <returns></returns>
        private DensityVolume CreateOrGetVolume()
        {
            GameObject DensityVolumeObject = GameObject.Find(DensityVolumeName);
            DensityVolume volume = FindObjectOfType<DensityVolume>();
            if (DensityVolumeObject == null)
            {
                if (volume == null)
                {
                    DensityVolumeObject = new GameObject(DensityVolumeName);
                    volume = DensityVolumeObject.AddComponent<DensityVolume>();
                }
            }
            else
            {
                volume = DensityVolumeObject.GetComponent<DensityVolume>();
            }

            return volume;
        }
        /// <summary>
        /// Updates the density volume transform
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        private void UpdateVolumeTransform(Vector3 position, Quaternion rotation)
        {
            m_volume.transform.SetPositionAndRotation(position, rotation);
        }

#endregion
#region Editor Utils

#if UNITY_EDITOR

        /// <summary>
        /// Gets the editor scene view camera
        /// </summary>
        /// <returns></returns>
        private Camera GetEditorSceneViewCamera()
        {
            Camera sceneCamera = null;
            if (SceneView.lastActiveSceneView != null)
            {
                sceneCamera = SceneView.lastActiveSceneView.camera;
            }

            return sceneCamera;
        }
        /// <summary>
        /// Editor update function that is called when the application is not playing
        /// </summary>
        /// <param name="camera"></param>
        private void EditorUpdateVolumeController()
        {
            if (m_editorMainCamera == null)
            {
                m_editorMainCamera = GetEditorSceneViewCamera();
                return;
            }

            if (m_volume == null)
            {
                return;
            }

            Vector3 position = m_editorMainCamera.transform.position;
            position += -m_editorMainCamera.transform.forward * 1.5f;
            UpdateVolumeTransform(position, Quaternion.identity);
            UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(m_volume, false);
        }

#endif

#endregion
#region Public Static Functions

        /// <summary>
        /// Global function to create the global density volume
        /// </summary>
        public static void CreateGaiaHDRPDensityVolume()
        {
            GameObject DensityVolumeObject = GameObject.Find(DensityVolumeName);
            if (DensityVolumeObject == null)
            {
                DensityVolume volume = FindObjectOfType<DensityVolume>();
                if (volume == null)
                {
                    DensityVolumeObject = new GameObject(DensityVolumeName);
                    DensityVolumeObject.AddComponent<DensityVolume>();
                }
                else
                {
                    DensityVolumeObject = volume.gameObject;
                    DensityVolumeObject.name = DensityVolumeName;
                }
            }
            else
            {
                DensityVolumeObject.GetComponent<DensityVolume>();
            }

            HDRPDensityVolumeController controller = DensityVolumeObject.GetComponent<HDRPDensityVolumeController>();
            if (controller == null)
            {
                controller = DensityVolumeObject.AddComponent<HDRPDensityVolumeController>();
            }

#if UNITY_EDITOR
            controller.DensityVolumeProfile.m_texture = AssetDatabase.LoadAssetAtPath<Texture3D>(GaiaUtils.GetAssetPath("Fog Noise Texture 3D.asset"));
#endif
            controller.DensityVolumeProfile.m_scrollSpeed = new Vector3(0f, 0f, 0.05f);
            Terrain terrain = Terrain.activeTerrain;
            float size = 512f;
            if (terrain != null)
            {
                size = terrain.terrainData.size.x;
        
            }
            controller.DensityVolumeProfile.m_tiling = new Vector3(size, size,size);
            controller.DensityVolumeProfile.m_size = new Vector3(40f, 40f, 40f);
            controller.ApplyChanges();

            GameObject gaiaRuntime = GameObject.Find(GaiaConstants.gaiaLightingObject);
            if (gaiaRuntime != null)
            {
                DensityVolumeObject.transform.SetParent(gaiaRuntime.transform);
            }
        }
        /// <summary>
        /// Removes Gaia HDRP Density Volume from the scene
        /// </summary>
        public static void RemoveGaiaHDRPDensityVolume()
        {
            if (Instance != null)
            {
                DestroyImmediate(Instance.gameObject);
            }
            else
            {
                HDRPDensityVolumeController densityVolume = FindObjectOfType<HDRPDensityVolumeController>();
                if (densityVolume != null)
                {
                    DestroyImmediate(densityVolume.gameObject);
                }
            }
        }
        /// <summary>
        /// Applies the changes
        /// </summary>
        /// <param name="profile"></param>
        public static HDRPDensityVolumeController ApplyChanges(GaiaLightingProfileValues profile)
        {
            if (Instance != null)
            {
                Instance.DensityVolumeProfile.m_effectType = profile.m_densityVolumeEffectType;
                Instance.DensityVolumeProfile.m_size = profile.m_customDensityVolumeEffectType;
                Instance.DensityVolumeProfile.m_resolution = profile.m_densityVolumeResolution;
                Instance.DensityVolumeProfile.m_tiling = profile.m_customDensityVolumeResolution;
                Instance.ApplyChanges();

                return Instance;
            }

            return null;
        }

#endregion
    }
}
#endif
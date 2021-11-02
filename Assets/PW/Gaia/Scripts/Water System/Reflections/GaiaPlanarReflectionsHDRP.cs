using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if HDPipeline
using UnityEngine.Rendering.HighDefinition;
#endif

namespace Gaia
{
    [ExecuteAlways]
    public class GaiaPlanarReflectionsHDRP : MonoBehaviour
    {
        #region Public Variables

        public bool RequestRender
        {
            get { return m_requestRender; }
            set
            {
                if (m_requestRender != value)
                {
                    m_requestRender = value;
                    RequestReflectionRender();
                }
            }
        }
        public bool m_renderEveryFrame = true;
        public bool m_usePositionCheck = true;
        public Camera m_mainCamera;
        public float m_reflectionIntenisty = 1f;

        #endregion
        #region Private Variables

#if HDPipeline && UNITY_2020_2_OR_NEWER
        public PlanarReflectionProbe m_reflections;
        private int m_notInBoundsLayerMask = 0;
        private bool m_notInBoundsHasBeenSet = false;
        private bool m_isInMaskBounds = true;
        private LayerMask m_currentLayerMasks;
#endif
        [SerializeField] private bool m_requestRender;
        private Vector3 m_lastPosition = Vector3.zero;
        private Vector3 m_lastRotation = Vector3.zero;

        #endregion
        #region Unity Functions

        private void OnEnable()
        {
#if HDPipeline && UNITY_2020_2_OR_NEWER
            if (m_reflections == null)
            {
                m_reflections = GetComponent<PlanarReflectionProbe>();
            }

            if (m_reflections != null)
            {
                m_reflections.realtimeMode = ProbeSettings.RealtimeMode.OnDemand;
                m_currentLayerMasks = m_reflections.settingsRaw.cameraSettings.culling.cullingMask;
                m_isInMaskBounds = true;
            }
#endif

            if (m_mainCamera == null)
            {
                m_mainCamera = GaiaUtils.GetCamera();
            }

#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
            if (!Application.isPlaying)
            {
                EditorApplication.update += EditorUpdate;
            }
#endif
        }
        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }
        private void OnDestroy()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }
        private void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (m_renderEveryFrame)
            {
                if (PositionChanged(m_mainCamera, m_usePositionCheck))
                {
                    RequestRender = true;
                }
            }
        }

        #endregion
        #region Public Functions

        /// <summary>
        /// Request a render on the reflections at it's current state
        /// </summary>
        public void RequestReflectionRender()
        {
            m_requestRender = false;
#if HDPipeline && UNITY_2020_2_OR_NEWER
            if (m_reflections != null)
            {
                if (!m_isInMaskBounds)
                {
                    if (!m_notInBoundsHasBeenSet)
                    {
                        m_reflections.settingsRaw.cameraSettings.culling.cullingMask = m_notInBoundsLayerMask;
                        m_reflections.RequestRenderNextUpdate();
                        m_notInBoundsHasBeenSet = true;
                    }

                    return;
                }
                else
                {
                    if (m_notInBoundsHasBeenSet)
                    {
                        m_reflections.settingsRaw.cameraSettings.culling.cullingMask = m_currentLayerMasks;
                        m_notInBoundsHasBeenSet = false;
                    }

                    m_currentLayerMasks = m_reflections.settingsRaw.cameraSettings.culling.cullingMask;
                }

                m_reflections.RequestRenderNextUpdate();
            }
#endif
        }
        /// <summary>
        /// Sets the reflection state
        /// </summary>
        /// <param name="state"></param>
        public void SetReflectionState(bool state)
        {
#if HDPipeline && UNITY_2020_2_OR_NEWER
            m_isInMaskBounds = state;
#endif
        }
        /// <summary>
        /// Sets if the probe is enabled or not
        /// </summary>
        /// <param name="active"></param>
        public void ReflectionsActive(bool active)
        {
#if HDPipeline && UNITY_2020_2_OR_NEWER
            if (m_reflections != null)
            {
                m_reflections.enabled = active;
            }
#endif
        }
        /// <summary>
        /// Sets the reflection intensity
        /// </summary>
        /// <param name="newValue"></param>
        public void UpdateReflectionIntensity(float newValue)
        {
#if HDPipeline && UNITY_2020_2_OR_NEWER
            if (m_reflections != null)
            {
                m_reflections.settingsRaw.lighting.multiplier = newValue;
            }
#endif
        }

        #endregion
        #region Private Functions

        /// <summary>
        /// Checks to see if the position has changed
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="overrideCheck"></param>
        /// <returns></returns>
        private bool PositionChanged(Camera camera, bool usePositionCheck = false)
        {
            if (camera == null)
            {
                Debug.LogError("The main camera is null. The position check will always return false if the camera value is null. Please make sure the camera value has been set if you're using 'Use Position Check'");
                return false;
            }

            if (!usePositionCheck)
            {
                return true;
            }
            else
            {
                Vector3 currentPostion = camera.transform.position;
                Vector3 currentRotation = camera.transform.eulerAngles;
                if (currentPostion != m_lastPosition || currentRotation != m_lastRotation)
                {
                    m_lastPosition = currentPostion;
                    m_lastRotation = currentRotation;
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Editor Update
        /// </summary>
        private void EditorUpdate()
        {
            if (m_renderEveryFrame)
            {
                RequestRender = true;
            }
        }

        #endregion
    }
}
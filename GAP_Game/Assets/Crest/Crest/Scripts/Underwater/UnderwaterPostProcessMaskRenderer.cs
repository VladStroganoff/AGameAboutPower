// Crest Ocean System for HDRP

// Copyright 2020 Wave Harmonic Ltd

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Crest
{
    internal class UnderwaterPostProcessMaskRenderer : CustomPass
    {
        static GameObject _gameObject;
        static readonly string _gameObjectName = "Ocean Mask";
        static readonly string _customPassName = "Crest Ocean Mask";

        const string SHADER_OCEAN_MASK = "Crest/Underwater/Ocean Mask";

        Shader _oceanMaskShader;
        Material _oceanMaskMaterial;
        RTHandle _colorBuffer;
        RTHandle _depthBuffer;
        Plane[] _cameraFrustumPlanes;

        public static UnderwaterPostProcessMaskRenderer Instance { get; private set; }

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            Instance = this;

            _oceanMaskShader = Shader.Find(SHADER_OCEAN_MASK);
            _oceanMaskMaterial = CoreUtils.CreateEngineMaterial(_oceanMaskShader);

            _colorBuffer = RTHandles.Alloc
            (
                scaleFactor: Vector2.one,
                slices: TextureXR.slices,
                dimension: TextureXR.dimension,
                colorFormat: GraphicsFormat.R16_SFloat,
                useDynamicScale: true,
                name: "Crest Ocean Mask"
            );

            _depthBuffer = RTHandles.Alloc
            (
                scaleFactor: Vector2.one,
                slices: TextureXR.slices,
                dimension: TextureXR.dimension,
                depthBufferBits: DepthBits.Depth24,
                colorFormat: GraphicsFormat.R8_UNorm, // This appears to be used for depth.
                enableRandomWrite: false,
                useDynamicScale: true,
                name: "Crest Ocean Mask Depth"
            );
        }

        protected override void Execute(ScriptableRenderContext renderContext, CommandBuffer commandBuffer,
            HDCamera hdCamera, CullingResults cullingResult)
        {
            if (OceanRenderer.Instance == null)
            {
                return;
            }

            var camera = hdCamera.camera;

            // Custom passes execute for every camera. We only support one camera for now.
            if (!ReferenceEquals(camera, OceanRenderer.Instance.ViewCamera))
            {
                return;
            }

            if (_cameraFrustumPlanes == null)
            {
                _cameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
            }

            GeometryUtility.CalculateFrustumPlanes(camera, _cameraFrustumPlanes);

            // It is more convenient to have this debug option on the post-processing component.
            var debugDisableOceanMask = false;
            if (UnderwaterPostProcessHDRP.Instance != null)
            {
                debugDisableOceanMask = UnderwaterPostProcessHDRP.Instance._disableOceanMask.value;
            }

            UnderwaterPostProcessUtils.PopulateOceanMask(commandBuffer, camera, OceanRenderer.Instance.Tiles,
                _cameraFrustumPlanes, _colorBuffer, _depthBuffer, _oceanMaskMaterial, debugDisableOceanMask);
        }

        protected override void Cleanup()
        {
            CoreUtils.Destroy(_oceanMaskMaterial);
            _colorBuffer.Release();
            _depthBuffer.Release();

            Instance = null;
        }

        public static void Enable()
        {
            // Find the existing custom pass volume.
            // During recompiles, the reference will be lost so we need to find the game object. It could be limited to
            // the editor if it is safe to do so. The last thing we want is leaking objects.
            if (_gameObject == null)
            {
                var transform = OceanRenderer.Instance.transform.Find(_gameObjectName);
                if (transform != null)
                {
                    _gameObject = transform.gameObject;
                }
            }

            // Create or update the custom pass volume.
            if (_gameObject == null)
            {
                _gameObject = new GameObject()
                {
                    name = _gameObjectName,
                    hideFlags = OceanRenderer.Instance._hideOceanTileGameObjects
                        ? HideFlags.HideAndDontSave : HideFlags.DontSave,
                };
                // Place the custom pass under the ocean renderer since it is easier to find later. Transform.Find can
                // find inactive game objects unlike GameObject.Find.
                _gameObject.transform.parent = OceanRenderer.Instance.transform;
                // It appears that this is currently the only way to add a custom pass.
                var volume = _gameObject.AddComponent<CustomPassVolume>();
                // NOTE: There is a bug which prevents us from using the same injection point for more than one volume.
                //       This only seems to be an issue with volumes create via scripting. It appears to be fixed in
                //       2020.1.
                volume.injectionPoint = CustomPassInjectionPoint.BeforeRendering;
                volume.isGlobal = true;
                volume.customPasses.Add(new UnderwaterPostProcessMaskRenderer()
                {
                    name = _customPassName,
                    targetColorBuffer = TargetBuffer.None,
                    targetDepthBuffer = TargetBuffer.None,
                });
            }
            else
            {
                _gameObject.hideFlags = OceanRenderer.Instance._hideOceanTileGameObjects
                        ? HideFlags.HideAndDontSave : HideFlags.DontSave;
                _gameObject.SetActive(true);
            }

        }

        public static void Disable()
        {
            // It should be safe to rely on this reference for this reference to fail.
            if (_gameObject != null)
            {
                _gameObject.SetActive(false);
            }
        }
    }
}

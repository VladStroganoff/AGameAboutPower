// Crest Ocean System for HDRP

// Copyright 2020 Wave Harmonic Ltd

using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static Crest.UnderwaterPostProcessUtils;

namespace Crest
{
    [VolumeComponentMenu("Crest/Underwater")]
    public class UnderwaterPostProcessHDRP : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public override CustomPostProcessInjectionPoint injectionPoint =>
#if UNITY_2020_2_OR_NEWER
            CustomPostProcessInjectionPoint.BeforeTAA;
#else
            CustomPostProcessInjectionPoint.BeforePostProcess;
#endif

        private Material _underwaterPostProcessMaterial;
        private PropertyWrapperMaterial _underwaterPostProcessMaterialWrapper;

        private const string SHADER = "Hidden/Crest/Underwater/Post Process HDRP";

        bool _firstRender = true;
        public BoolParameter _enable = new BoolParameter(true);
        public BoolParameter _copyOceanMaterialParamsEachFrame = new BoolParameter(false);
        // This adds an offset to the cascade index when sampling ocean data, in effect smoothing/blurring it. Default
        // to shifting the maximum amount (shift from lod 0 to penultimate lod - dont use last lod as it cross-fades
        // data in/out), as more filtering was better in testing.
        [Tooltip(UnderwaterPostProcessUtils.tooltipFilterOceanData)]
        public ClampedIntParameter _filterOceanData = new ClampedIntParameter(UnderwaterPostProcessUtils.DefaultFilterOceanDataValue, UnderwaterPostProcessUtils.MinFilterOceanDataValue, UnderwaterPostProcessUtils.MaxFilterOceanDataValue);

        [Tooltip(tooltipMeniscus)]
        public BoolParameter _meniscus = new BoolParameter(true);

        [Header("Debug Options")]
        public BoolParameter _viewOceanMask = new BoolParameter(false);
        public BoolParameter _disableOceanMask = new BoolParameter(false);

        [Tooltip("Adjusts the far plane for horizon line calculation. Helps with horizon line issue. (Experimental)")]
        public ClampedFloatParameter _farPlaneMultiplier = new ClampedFloatParameter(0.68f, 0f, 1f);

        [Tooltip("Use the old horizon safety margin multiplier to fix horizon line issues instead of the new experimental far plane multiplier.")]
        public BoolParameter _useHorizonSafetyMarginMultiplier = new BoolParameter(false);

        [Tooltip(UnderwaterPostProcessUtils.tooltipHorizonSafetyMarginMultiplier)]
        public ClampedFloatParameter _horizonSafetyMarginMultiplier = new ClampedFloatParameter(UnderwaterPostProcessUtils.DefaultHorizonSafetyMarginMultiplier, 0f, 1f);
        public BoolParameter _scaleSafetyMarginWithDynamicResolution = new BoolParameter(true);

        UnderwaterSphericalHarmonicsData _sphericalHarmonicsData = new UnderwaterSphericalHarmonicsData();
        Camera _camera;

        public static UnderwaterPostProcessHDRP Instance { get; private set; }

        public bool IsActive()
        {
            if (!Application.isPlaying)
            {
                return false;
            }
            if (OceanRenderer.Instance != null)
            {
                return OceanRenderer.Instance.ViewerHeightAboveWater < 2f && _enable.value;
            }
            else
            {
                return false;
            }
        }

        public override void Setup()
        {
            Instance = this;

            _underwaterPostProcessMaterial = CoreUtils.CreateEngineMaterial(SHADER);
            _underwaterPostProcessMaterialWrapper = new PropertyWrapperMaterial(_underwaterPostProcessMaterial);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(_underwaterPostProcessMaterial);

            // Guard against setup/clean-up overlap.
            if (Instance == this)
            {
                Instance = null;
                UnderwaterPostProcessMaskRenderer.Disable();
            }
        }

        public override void Render(CommandBuffer commandBuffer, HDCamera camera, RTHandle source, RTHandle destination)
        {
            // Used for cleanup
            _camera = camera.camera;

            // TODO: Put somewhere else?
            XRHelpers.Update(_camera);

            if (OceanRenderer.Instance == null)
            {
                HDUtils.BlitCameraTexture(commandBuffer, source, destination);
                return;
            }

            // Applying the post-processing effect to the scene camera doesn't
            // work well.
            if (camera.camera.gameObject.name == "SceneCamera")
            {
                HDUtils.BlitCameraTexture(commandBuffer, source, destination);
                return;
            }

            if (UnderwaterPostProcessMaskRenderer.Instance == null)
            {
                UnderwaterPostProcessMaskRenderer.Enable();
            }

            // Dynamic resolution will cause the horizon gap to widen. For extreme values, the gap can be several pixels
            // thick. It can be disabled as it will require thorough testing across different hardware to get it 100%
            // right. This doesn't appear to be an issue for the built-in renderer.
            var horizonSafetyMarginMultiplier = _horizonSafetyMarginMultiplier.value;
            if (_scaleSafetyMarginWithDynamicResolution.value && DynamicResolutionHandler.instance.DynamicResolutionEnabled())
            {
                // 100 is a magic number from testing. Works well with default horizonSafetyMarginMultiplier of 0.01.
                horizonSafetyMarginMultiplier = Mathf.Lerp(horizonSafetyMarginMultiplier * 100,
                    horizonSafetyMarginMultiplier, DynamicResolutionHandler.instance.GetCurrentScale());
            }

            UpdatePostProcessMaterial(
                source,
                camera.camera,
                _underwaterPostProcessMaterialWrapper,
                _sphericalHarmonicsData,
                _meniscus.value,
                _firstRender || _copyOceanMaterialParamsEachFrame.value,
                _viewOceanMask.value,
                // horizonSafetyMarginMultiplier is added to the horizon, so no-op is zero.
                _useHorizonSafetyMarginMultiplier.value ? horizonSafetyMarginMultiplier : 0f,
                // farPlaneMultiplier is multiplied to the far plane, so no-op is one.
                _useHorizonSafetyMarginMultiplier.value ? 1f : _farPlaneMultiplier.value,
                _filterOceanData.value
            );

            HDUtils.DrawFullScreen(commandBuffer, _underwaterPostProcessMaterial, destination);

            _firstRender = false;
        }
    }
}

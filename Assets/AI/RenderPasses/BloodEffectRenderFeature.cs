using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BloodEffectRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class BloodEffectSettings
    {
        public Material bloodEffectMaterial;
        public RenderTexture bloodAccumulationTexture;
    }

    public BloodEffectSettings settings = new BloodEffectSettings();
    private BloodEffectRenderPass bloodEffectPass;
    public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    public override void Create()
    {
        bloodEffectPass = new BloodEffectRenderPass(settings.bloodEffectMaterial, settings.bloodAccumulationTexture);
        bloodEffectPass.renderPassEvent = renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        Debug.Log("AddRenderPasses"); if (settings.bloodEffectMaterial == null || settings.bloodAccumulationTexture == null) return;

         renderer.EnqueuePass(bloodEffectPass);
    }
}
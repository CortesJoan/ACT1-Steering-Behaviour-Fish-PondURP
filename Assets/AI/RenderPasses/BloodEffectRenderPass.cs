using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloodEffectRenderPass : ScriptableRenderPass
{
    private Material bloodEffectMaterial;
    private RenderTexture bloodAccumulationTexture;
 
    public BloodEffectRenderPass(Material bloodEffectMaterial, RenderTexture bloodAccumulationTexture)
    {
        this.bloodEffectMaterial = bloodEffectMaterial;
        this.bloodAccumulationTexture = bloodAccumulationTexture;
    }
 

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {

        if (bloodEffectMaterial == null) return;
        Debug.Log("Execute");
        CommandBuffer cmd = CommandBufferPool.Get("Blood Effect");
        cmd.SetRenderTarget(renderingData.cameraData.renderer.cameraColorTargetHandle);
        cmd.ClearRenderTarget(true, true, Color.clear);
        cmd.SetGlobalTexture("_BloodAccumulation", bloodAccumulationTexture);

  
        // Blit the camera's color target to itself, applying the blood effect
        RenderTargetIdentifier cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        int tempID = Shader.PropertyToID("TempBloodEffectRT"); // Assign unique ID

        // Create a temporary Render Texture
        cmd.GetTemporaryRT(tempID, descriptor, FilterMode.Bilinear);

        // Blit to the temp Render Texture first
        cmd.Blit(cameraColorTarget, tempID, bloodEffectMaterial);

        // Blit the temp RT to the camera target
        cmd.Blit(tempID, cameraColorTarget);

        cmd.ReleaseTemporaryRT(tempID); // Release the temp RT
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
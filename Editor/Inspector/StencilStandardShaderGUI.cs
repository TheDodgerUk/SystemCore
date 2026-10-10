using UnityEditor;

public class StencilStandardShaderGUI : StandardShaderGUI
{
    private MaterialProperty m_StencilID;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        base.OnGUI(materialEditor, props);

        DrawProperty(materialEditor, props, "_Stencil", "Stencil ID");
        DrawProperty(materialEditor, props, "_StencilComp", "Stencil Comparison");
        DrawProperty(materialEditor, props, "_StencilOp", "Stencil Operation");
        DrawProperty(materialEditor, props, "_StencilWriteMask", "Stencil Write Mask");
        DrawProperty(materialEditor, props, "_StencilReadMask", "Stencil Read Mask");
        DrawProperty(materialEditor, props, "_ColorMask", "Colour Mask");
    }

    private void DrawProperty(MaterialEditor materialEditor, MaterialProperty[] props, string name, string displayName)
    {
        materialEditor.ShaderProperty(FindProperty(name, props), displayName);
    }
}

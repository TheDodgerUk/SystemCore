using UnityEngine;

public static class PhysicMaterials
{
    public static void Initialise()
    {
        // this is to Initialise all the readonly data below
        var temp = HandPhysicMaterial;
    }

    public static readonly PhysicMaterial HandPhysicMaterial = Resources.Load<PhysicMaterial>("PhysicsMaterials/Hand Physics Material");
    public static readonly PhysicMaterial BouncyPhysicMaterial = Resources.Load<PhysicMaterial>("PhysicsMaterials/Bouncy");
    public static readonly PhysicMaterial JengaPhysicMaterial = Resources.Load<PhysicMaterial>("PhysicsMaterials/Jenga");
    public static readonly PhysicMaterial ButtonSliderPhysicMaterial = Resources.Load<PhysicMaterial>("PhysicsMaterials/ButtonSlider");
    public static readonly PhysicMaterial UIPhysicMaterial = Resources.Load<PhysicMaterial>("PhysicsMaterials/UI");

    public static readonly PhysicMaterial NoFriction = Resources.Load<PhysicMaterial>("NoFriction");
    public static readonly PhysicMaterial SlightFriction = Resources.Load<PhysicMaterial>("SlightFriction");


}

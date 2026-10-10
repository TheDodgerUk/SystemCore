using UnityEngine;

public static class PhysicMaterials
{
    public static void Initialise()
    {
        // this is to Initialise all the readonly data below
        var temp = HandPhysicMaterial;
    }

    public static readonly PhysicsMaterial HandPhysicMaterial = Resources.Load<PhysicsMaterial>("PhysicsMaterials/Hand Physics Material");
    public static readonly PhysicsMaterial BouncyPhysicMaterial = Resources.Load<PhysicsMaterial>("PhysicsMaterials/Bouncy");
    public static readonly PhysicsMaterial JengaPhysicMaterial = Resources.Load<PhysicsMaterial>("PhysicsMaterials/Jenga");
    public static readonly PhysicsMaterial ButtonSliderPhysicMaterial = Resources.Load<PhysicsMaterial>("PhysicsMaterials/ButtonSlider");
    public static readonly PhysicsMaterial UIPhysicMaterial = Resources.Load<PhysicsMaterial>("PhysicsMaterials/UI");

    public static readonly PhysicsMaterial NoFriction = Resources.Load<PhysicsMaterial>("NoFriction");
    public static readonly PhysicsMaterial SlightFriction = Resources.Load<PhysicsMaterial>("SlightFriction");


}

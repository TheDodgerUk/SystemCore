using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCatalogueInteractions 
{

    public void AddInteractions(GameObject objRoot, CatalogueEntry entry, ref SimpleDictionaryList<MetaDataType, VrInteraction> newList, ref SimpleDictionaryList<MetaDataType, MetaData> metaData)
    {
        newList = new SimpleDictionaryList<MetaDataType, VrInteraction>();
        metaData = new SimpleDictionaryList<MetaDataType, MetaData>();


        foreach (var item in entry.GetLoadedMetaData())
        {
            var newJsonData = Json.FullSerialiser.WriteToText((MetaData)item, true);
            var newData = Json.FullSerialiser.ReadFromText<MetaData>(newJsonData);
            Type itemType = item.GetType();

            if (itemType == typeof(ContentSliderMetaData))
            {
                var sliders = SliderMetaData(objRoot, newData);
                newList.AddToList(MetaDataType.ContentSlider, sliders);
                metaData.AddToList(MetaDataType.ContentSlider, newData);
            }
            else if (itemType == typeof(ArFeaturesMetaData))
            {
                var features = ArFeaturesData(objRoot, newData);
                newList.AddToList(MetaDataType.ArFeatures, features);
                metaData.AddToList(MetaDataType.ArFeatures, newData);
            }
            else if (itemType == typeof(ContentButtonMetaData))
            {
                var buttons = ButtonMetaData(objRoot, newData);
                newList.AddToList(MetaDataType.ContentButton, buttons);
                metaData.AddToList(MetaDataType.ContentButton, newData);
            }
            else if (itemType == typeof(ContentDialMetaData))
            {
                var dials = DialMetaData(objRoot, newData);
                newList.AddToList(MetaDataType.ContentDial, dials);
                metaData.AddToList(MetaDataType.ContentDial, newData);
            }
            else if (itemType == typeof(ContentClickMetaData))
            {
                var dials = ClickMetaData(objRoot, newData);
                newList.AddToList(MetaDataType.ContentClick, dials);
                metaData.AddToList(MetaDataType.ContentClick, newData);
            }
            else if (itemType == typeof(ContentMaterialChangeMetaData))
            {
                var mat = MaterailMetaData(objRoot, newData);
                newList.AddToList(MetaDataType.ContentMaterialChange, mat);
                metaData.AddToList(MetaDataType.ContentMaterialChange, newData);
            }
            if (itemType == typeof(ContentPickUpSocketMetaData))
            {
                var pickup = PickUpSocketMetaData(objRoot, newData);
                newList.AddToList(MetaDataType.ContentPickUpSocket, pickup);
                metaData.AddToList(MetaDataType.ContentPickUpSocket, newData);
            }
            if (itemType == typeof(ContentPickUpCableMetaData))
            {
                var pickup = PickUpCableMetaData(objRoot, newData);
                newList.AddToList(MetaDataType.ContentPickUpCable, pickup);
                metaData.AddToList(MetaDataType.ContentPickUpCable, newData);
            }
            if (itemType == typeof(ContentHingeMetaData))
            {
                var pickup = HingeMetaData(objRoot, newData);
                newList.AddToList(MetaDataType.ContentHinge, pickup);
                metaData.AddToList(MetaDataType.ContentHinge, newData);
            }

            if (itemType == typeof(ContentCollisionMetaData))
            {
                var pickup = CollisionMetaData(objRoot, newData);
                newList.AddToList(MetaDataType.ContentCollision, pickup);
                metaData.AddToList(MetaDataType.ContentCollision, newData);
            }

        }

        // this applys to the root objects
        foreach (var item in entry.GetLoadedMetaData())
        {
            var newJsonData = Json.FullSerialiser.WriteToText((MetaData)item, true);
            var newData = Json.FullSerialiser.ReadFromText<MetaData>(newJsonData);
            Type itemType = item.GetType();


            if (itemType == typeof(ContentPickUpMetaData))
            {
                var pickup = PickUpMetaData(objRoot, newData);
                newList.AddToList(MetaDataType.ContentPickUp, pickup);
                metaData.AddToList(MetaDataType.ContentPickUp, newData);
            }
            else if (itemType == typeof(ContentFoodMetaData))
            {
                var food = PickFoodMetaData(objRoot, newData);
                newList.AddToList(MetaDataType.ContentFood, food);
                metaData.AddToList(MetaDataType.ContentFood, newData);
            }
            else if (itemType == typeof(ContentDrinkMetaData))
            {
                var drink = PickDrinkMetaData(objRoot, newData);
                newList.AddToList(MetaDataType.ContentDrink, drink);
                metaData.AddToList(MetaDataType.ContentDrink, newData);
            }
            else if (itemType == typeof(ContentScriptMetaData))
            {
                var dials = ScriptMetaData(objRoot, newData);
                newList.AddToList(MetaDataType.ContentScript, dials);
                metaData.AddToList(MetaDataType.ContentScript, newData);
            }
        }

        if(metaData.KeyExists(MetaDataType.ContentPickUp) == true && metaData.KeyExists(MetaDataType.ContentDrink) == true)
        {
            Debug.LogError($"{entry.FullName} , ContentPickUp and ContentDrink cannot be on same item, remove ContentPickUp");
        }

        if (metaData.KeyExists(MetaDataType.ContentPickUp) == true && metaData.KeyExists(MetaDataType.ContentFood) == true)
        {
            Debug.LogError($"{entry.FullName} , ContentPickUp and ContentFood cannot be on same item, remove ContentPickUp");
        }

    }

    private List<VrInteraction> SliderMetaData(GameObject obj, MetaData newData)
    {
        if (Core.Environment.CurrentEnvironment.InteractionType == GlobalConsts.InteractionType.Physics)
        {
            return SliderMetaDataPhysics(obj, newData);
        }
        else
        {
            return SliderMetaDataNonPhysics(obj, newData);
        }
    }

    private List<VrInteraction> SliderMetaDataNonPhysics(GameObject obj, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentSliderMetaData data = (ContentSliderMetaData)newData;
        data.CollectAllData(obj);

        for (int i = 0; i < data.m_ItemData.Count; i++)
        {
            int index = i;
            ContentSliderMetaData.ItemData sliderItemData = data.m_ItemData[index];

            Collider col = sliderItemData.m_ModelGameObject.GetComponent<Collider>();
            if (col == null)
            {
                col = sliderItemData.m_ModelGameObject.ForceComponent<BoxCollider>();
            }
            col.isTrigger = true;

            var slider = sliderItemData.m_ModelGameObject.ForceComponent<VrInteractionSlider>();

            slider.Initialise(obj, sliderItemData, null);
            newList.Add(slider);
        }

        return newList;
    }

    private void ConvertSliderToVR(GameObject root, ContentSliderMetaData.ItemData sliderItemData, ref GameObject child)
    {
        Vector3 startPosition = root.transform.localPosition;
        Vector3 endPosition = Vector3.zero;
        VrInteractionSlider.CollectData(root, ref endPosition, sliderItemData);
        root.transform.localPosition = Vector3.Lerp(startPosition, endPosition, 0.5f);
        ConfigurableJoint joint = null;
        BasicSetupVr(root, ref child, ref joint);

        joint.zMotion = ConfigurableJointMotion.Limited;



        var lin = joint.linearLimit;
        lin.limit = Vector3.Distance(startPosition, endPosition) / 2f;
        joint.linearLimit = lin;

        var drive = joint.zDrive;
        drive.positionSpring = 0f;
        drive.positionDamper = 0f;
        joint.zDrive = drive;
    }

    private void BasicSetupVr(GameObject root, ref GameObject child, ref ConfigurableJoint joint)
    {
        child = GameObject.Instantiate(root);
        child.name = child.name.Replace("(Clone)", "");
        child.transform.SetParent(root.transform);
        child.transform.ClearLocals();

        var allComponants = root.transform.parent.GetComponents<Component>();
        if (allComponants.Length != 1)
        {
            // this is pfr parenting and placing start and end positions corretly 
            DebugBeep.LogError($"for slider to work in VR, {root.name} must have a parent which is empty and in line with {root.name}", DebugBeep.MessageLevel.High);
        }
        root.name = root.name.Replace("LOD0", "");
        root.name += "_Connection";
        root.name = root.name.Replace("__", "_");


        //if (root)

        var rootRB = root.ForceComponent<Rigidbody>();
        rootRB.constraints = RigidbodyConstraints.FreezeAll;
        rootRB.useGravity = false;
        rootRB.isKinematic = true;

        var col = root.GetComponent<Collider>();
        if (col != null)
        {
            UnityEngine.Object.Destroy(col);
        }
        var rootRenderer = root.ForceComponent<Renderer>();
        rootRenderer.enabled = false;


        var childRB = child.ForceComponent<Rigidbody>();
        childRB.constraints = RigidbodyConstraints.FreezeRotation;
        childRB.useGravity = false;
        childRB.isKinematic = false;
        joint = childRB.gameObject.ForceComponent<ConfigurableJoint>();
        joint.connectedBody = rootRB;
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = Vector3.zero;
        joint.anchor = Vector3.zero;

        for (int j = root.transform.childCount - 1; j >= 0; j--)
        {
            GameObject toDelete = root.transform.GetChild(j).gameObject;
            if (toDelete != child.gameObject)
            {
                UnityEngine.Object.DestroyImmediate(toDelete);
            }
        }
    }

    private List<VrInteraction> SliderMetaDataPhysics(GameObject obj, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentSliderMetaData data = (ContentSliderMetaData)newData;
        data.CollectAllData(obj);

        for (int i = 0; i < data.m_ItemData.Count; i++)
        {
            int index = i;
            ContentSliderMetaData.ItemData sliderItemData = data.m_ItemData[index];

            GameObject child = null;
            ConvertSliderToVR(sliderItemData.m_ModelGameObject, sliderItemData, ref child);

            if (child.GetComponent<Collider>() == null)
            {
                var col = child.ForceComponent<BoxCollider>();
            }
            var slider = child.ForceComponent<VrInteractionPhysicalSlider>();
            slider.Initialise(obj, sliderItemData, null);
            // this needs to be done after VrInteractionPhysicalButton
            var childRB = child.ForceComponent<Rigidbody>();
            childRB.constraints = RigidbodyConstraints.FreezeRotation;
            childRB.useGravity = false;
            childRB.isKinematic = false;
            childRB.drag = 100;
            newList.Add(slider);
        }

        return newList;
    }


    

    private List<VrInteraction> ArFeaturesData(GameObject obj, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ArFeaturesMetaData data = (ArFeaturesMetaData)newData;
        data.CollectAllData(obj);

        if(data.m_bSupportsExplosion == true)
        {
            ExplodeProduct explode = obj.AddComponent<ExplodeProduct>();
            explode.TriggerResetProduct();
        }
        if(data.m_bSupportsXRay == true)
        {
            XrayProduct xray = obj.AddComponent<XrayProduct>();
            xray.m_XrayRenderers = new Renderer[data.m_XrayGameObjects.Count];
            for (int i = 0; i < data.m_XrayGameObjects.Count; i++)
            {
                xray.m_XrayRenderers[i] = data.m_XrayGameObjects[i].GetComponent<Renderer>();
            }
            xray.Initialise();
        }
        return newList;
    }

    private List<VrInteraction> ButtonMetaData(GameObject obj, MetaData newData)
    {
        if (Core.Environment.CurrentEnvironment.InteractionType == GlobalConsts.InteractionType.Physics)
        {
            return ButtonMetaDataPhysics(obj, newData);          
        }
        else
        {
            return ButtonMetaDataNonPhysics(obj, newData);
        }
    }

    #region ButtonData
    private List<VrInteraction> ButtonMetaDataNonPhysics(GameObject obj, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentButtonMetaData data = (ContentButtonMetaData)newData;
        data.CollectAllData(obj);

        for (int i = 0; i < data.m_ItemData.Count; i++)
        {
            int index = i;
            ContentButtonMetaData.ButtonItemData buttonItemData = data.m_ItemData[index];

            Collider col = buttonItemData.m_ModelGameObject.GetComponent<Collider>();
            if(col == null)
            {
                col = buttonItemData.m_ModelGameObject.ForceComponent<BoxCollider>();
            }
            col.isTrigger = true;

            switch (buttonItemData.m_ButtonType)
            {
                case ContentButtonMetaData.ButtonItemData.ButtonType.Toggle:
                    var buttonup = buttonItemData.m_ModelGameObject.ForceComponent<VrInteractionButtonToggle>();
                    buttonup.Initialise(obj, buttonItemData, null);
                    newList.Add(buttonup);
                    break;
                case ContentButtonMetaData.ButtonItemData.ButtonType.Hold:
                    var buttonhold = buttonItemData.m_ModelGameObject.ForceComponent<VrInteractionButtonHold>();
                    buttonhold.Initialise(obj, buttonItemData, null);
                    newList.Add(buttonhold);
                    break;
                case ContentButtonMetaData.ButtonItemData.ButtonType.Latched:
                    var button = buttonItemData.m_ModelGameObject.ForceComponent<VrInteractionButtonLatched>();
                    button.Initialise(obj, buttonItemData, null);
                    newList.Add(button);
                    break;
                default:
                    break;
            }


        }
        return newList;
    }

    private List<VrInteraction> ButtonMetaDataPhysics(GameObject obj, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentButtonMetaData data = (ContentButtonMetaData)newData;
        data.CollectAllData(obj);

        for (int i = 0; i < data.m_ItemData.Count; i++)
        {
            int index = i;
            ContentButtonMetaData.ButtonItemData buttonItemData = data.m_ItemData[index];

            GameObject child = null;
            if (buttonItemData.m_ModelGameObject != null)
            {
                ConvertButtonToVR(buttonItemData.m_ModelGameObject, buttonItemData, ref child);
            }
            else
            {
                Debug.LogError($"Button error on obj : {obj.GetGameObjectPath()}");
                continue;
            }


            if (child.GetComponent<Collider>() == null)
            {
                // if no collider add box one
                var col = child.ForceComponent<BoxCollider>();
            }

            var buttonup = child.ForceComponent<VrInteractionPhysicalButton>();
            buttonup.Initialise(buttonItemData, null);

            // this needs to be done after VrInteractionPhysicalButton
            var childRB = child.ForceComponent<Rigidbody>();
            childRB.constraints = RigidbodyConstraints.FreezeRotation;
            childRB.useGravity = false;
            childRB.isKinematic = false;


            newList.Add(buttonup);

        }
        return newList;
    }

    #endregion

    private List<VrInteraction> DialMetaData(GameObject obj, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentDialMetaData data = (ContentDialMetaData)newData;
        data.CollectAllData(obj);


        for (int i = 0; i < data.m_DialData.Count; i++)
        {
            int index = i;
            ContentDialMetaData.StringItemData dialItemData = data.m_DialData[index];

            var col = dialItemData.m_ModelGameObject.ForceComponent<BoxCollider>();
            col.isTrigger = true;

            var dial = dialItemData.m_ModelGameObject.ForceComponent<VrInteractionDial>();


            dial.Initialise(obj, dialItemData, null);
            newList.Add(dial);
        }
        return newList;
    }


    private List<VrInteraction> ClickMetaData(GameObject obj, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentClickMetaData data = (ContentClickMetaData)newData;
        data.CollectAllData(obj);


        for (int i = 0; i < data.m_ClickData.Count; i++)
        {
            int index = i;
            InstructionData clickItemData = data.m_ClickData[index];

            var col = clickItemData.m_ModelGameObject.ForceComponent<BoxCollider>();
            col.isTrigger = true;

            var click = clickItemData.m_ModelGameObject.ForceComponent<VrInteractionClickCallBack>();

            click.Initialise(obj, clickItemData, null);
            newList.Add(click);
        }
        return newList;
    }

    private List<VrInteraction> ScriptMetaData(GameObject obj, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentScriptMetaData data = (ContentScriptMetaData)newData;
        data.CollectAllData(obj);

        data.AddItem(obj);     
        return newList;
    }

    private List<VrInteraction> PickUpMetaData(GameObject obj, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentPickUpMetaData data = (ContentPickUpMetaData)newData;
        data.CollectAllData(obj);

        var pickUp = obj.ForceComponent<VrInteractionPickUp>();
        pickUp.Initialise(data);
        newList.Add(pickUp);
        return newList;
    }

    private List<VrInteraction> PickUpCableMetaData(GameObject obj, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentPickUpCableMetaData data = (ContentPickUpCableMetaData)newData;
        data.CollectAllData(obj);

        for (int i = 0; i < data.m_Data.Count; i++)
        {
            var pickUp = data.m_Data[i].m_GameObjectPickup.ForceComponent<VrInteractionPickUpCable>();
            pickUp.Initialise(obj, data.m_Data[i]);
            newList.Add(pickUp);
        }

        return newList;
    }

    private List<VrInteraction> PickUpSocketMetaData(GameObject obj, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentPickUpSocketMetaData data = (ContentPickUpSocketMetaData)newData;
        data.CollectAllData(obj);

        for (int i = 0; i < data.m_Data.Count; i++)
        {
            var pickUp = data.m_Data[i].m_GameObjectPickup.ForceComponent<VrInteractionPickUpSocket>();
            pickUp.Initialise(obj, data.m_Data[i]);
            newList.Add(pickUp);
        }

        return newList;
    }



    private List<VrInteraction> PickDrinkMetaData(GameObject obj, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentDrinkMetaData data = (ContentDrinkMetaData)newData;
        data.CollectAllData(obj);

        var drink = obj.ForceComponent<VrInteractionDrink>();
        drink.Initialise(data);
        newList.Add(drink);
        return newList;
    }

    private List<VrInteraction> PickFoodMetaData(GameObject obj, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentFoodMetaData data = (ContentFoodMetaData)newData;
        data.CollectAllData(obj);

        var food = obj.ForceComponent<VrInteractionFood>();
        food.Initialise(data);
        newList.Add(food);
        return newList;
    }

    private List<VrInteraction> HingeMetaData(GameObject objRoot, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentHingeMetaData data = (ContentHingeMetaData)newData;
        data.CollectAllData(objRoot);

        foreach (var item in data.m_HingeData)
        {
            var hinge = item.m_HingeGameObject.ForceComponent<VrInteractionHinge>();
            hinge.Initialise(objRoot, item);
            newList.Add(hinge);
        }

        return newList;
    }

    private List<VrInteraction> CollisionMetaData(GameObject objRoot, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentCollisionMetaData data = (ContentCollisionMetaData)newData;
        data.CollectAllData(objRoot);

        foreach (var item in data.m_CollisionData)
        {
            var hinge = item.m_Colliders.GameObjectRef.ForceComponent<VrInteractionCollision>();
            hinge.Initialise(objRoot, item);
            newList.Add(hinge);
        }

        return newList;
    }

    

    private List<VrInteraction> MaterailMetaData(GameObject obj, MetaData newData)
    {
        List<VrInteraction> newList = new List<VrInteraction>();
        ContentMaterialChangeMetaData data = (ContentMaterialChangeMetaData)newData;
        data.CollectAllData(obj);
        return newList;
    }


    private void ConvertButtonToVR(GameObject root, ContentButtonMetaData.ButtonItemData buttonItemData, ref GameObject child)
    {
        ConfigurableJoint joint = null;
        BasicSetupVr(root, ref child, ref joint);

        joint.yMotion = ConfigurableJointMotion.Limited;

        var lin = joint.linearLimit;
        lin.limit = Math.Abs(buttonItemData.m_ModelEndLocalPositionfloat);
        joint.linearLimit = lin;

        var drive = joint.yDrive;
        drive.positionSpring = 50;
        drive.positionDamper = 10;
        joint.yDrive = drive;
    }
}

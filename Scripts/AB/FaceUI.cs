using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FaceUI : ScreenUI
{
    private const string ANIMATION_ROOT = "Bounce/ABBot";
    private const string NECK_BONE = "BN_Hub/BN_Neck";

    private GameObject m_FaceObject;
    private Transform m_HeadTarget;
    private Transform m_EyeTarget;
    private Vector3 m_PriorityTarget;
    private EyeEmotion m_EmotionState;

    [Range(0, 1)]
    public float m_PriorityTargetBlend = 0f;
    [Range(0, 1)]
    public float m_EyeLookBlend = 0f;
    [SerializeField]
    private float m_fMaxHorizontalLook = 0.11f;
    [SerializeField]
    private float m_fMaxVerticalLook = 0.11f;
    private Transform[] Eyes;

    private float m_fMinEyeSeparation = 0.4f;
    private float m_fMaxEyeSeparation = 0.6f;

    public bool bTrackTarget = true;
    private Transform HeadPosition;
    private Transform m_HeadTransform;

    public EyeEmotion GetFacialEmotion => m_EmotionState;
    public Transform GetHeadTransform => m_HeadTransform;

    private Vector3 targetDirection;
    private Vector3 lookPosition;

    private BoneLookAt lookAt;
    [SerializeField]
    private float m_fSmoothing = 2.5f;

    public void SetPriorityTarget(Vector3 target)
    {
        m_PriorityTarget = target;
    }

    public override void Init(Material mat)
    {
        base.Init(mat);

        m_HeadTransform = transform.Search("HeadFaceDir");
        CreateFaceAnimator();
        SetupScreen(mat);

        Transform AnimationRoot = transform.Find(ANIMATION_ROOT);
        if (null != AnimationRoot)
        {
            Transform LookBone = AnimationRoot.Find(NECK_BONE);
            lookAt = AnimationRoot.gameObject.ForceComponent<BoneLookAt>();
            lookAt.Init(LookBone, new Vector3(0f, -90f, -90f));
        }
    }

    private void SetupScreen(Material faceMat)
    {
        faceMat.SetTexture("_MainTex", m_RenderTexture);
        faceMat.SetTextureScale("_MainTex", new Vector2(1f, 1f));
        faceMat.SetTextureOffset("_MainTex", new Vector2(-0.03f, 0.08f));

        m_HeadTarget = null;
    }

    private void CreateFaceAnimator()
    {
        m_FaceObject = m_CameraTransform.transform.Find("RobotFace").gameObject;
        SetLayerRecursively(m_FaceObject, m_LayerMask);
        m_FaceObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        m_FaceObject.layer = m_LayerMask;
        m_FaceObject.transform.localPosition = Vector3.forward * 0.2f;

        Eyes = new Transform[2];
        Eyes[0] = m_FaceObject.transform.Find("LeftEye");
        Eyes[1] = m_FaceObject.transform.Find("RightEye");

        m_EmotionState = m_FaceObject.ForceComponent<EyeEmotion>();
        m_EmotionState.SetEmotion(ABEmotion.Normal);
    }

    private void Update()
    {
        if (false == bTrackTarget)
        {
            if (null != m_FaceObject)
            {
                Vector3 position = m_FaceObject.transform.localPosition;
                position.x = 0f;
                position.y = 0f;
                LerpToPostion(position);
            }
            return;
        }

        lookPosition = GetLookTarget();
        lookAt.SetLookTarget(lookPosition);

        //Get Angle from forward to target
        targetDirection = (lookPosition - m_HeadTransform.position).normalized;

        Vector3 localDir = m_HeadTransform.InverseTransformDirection(targetDirection);
        if (localDir.z > 0f)
        {
            Vector3 position = m_FaceObject.transform.localPosition;
            position.x = -localDir.x * m_fMaxHorizontalLook;
            position.y = localDir.y * m_fMaxVerticalLook;
            LerpToPostion(position);
        }
    }

    private void LerpToPostion(Vector3 position)
    {
        Vector3 currentPos = m_FaceObject.transform.localPosition;
        m_FaceObject.transform.localPosition = Vector3.Lerp(currentPos, position, Time.deltaTime * m_fSmoothing);
    }

    private Vector3 GetLookTarget()
    {
        Vector3 position = GetLookBlendPosition(m_HeadTarget, m_EyeTarget, m_EyeLookBlend);
        position = Vector3.Lerp(position, m_PriorityTarget, m_PriorityTargetBlend);

        return position;
    }

    private Vector3 GetLookBlendPosition(Transform A, Transform B, float normalised)
    {
        if (null == A)
        {
            return Vector3.zero;
        }

        if (null == B)
        {
            return A.position;
        }

        return Vector3.Lerp(A.position, B.position, normalised);
    }

    public void SetLookTarget(Transform headTarget, Transform eyeTarget)
    {
        m_HeadTarget = headTarget;
        m_EyeTarget = eyeTarget;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(lookPosition, 0.1f);
    }
}
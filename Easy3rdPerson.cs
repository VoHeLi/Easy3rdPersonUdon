
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]

public class Easy3rdPerson : UdonSharpBehaviour
{

    [Header("-----Internal, don't touch-----")]

    [SerializeField] private RectTransform displayImageTransform;
    [SerializeField] private RawImage displayImage;
    [SerializeField] private RawImage displayImageUI;
    [SerializeField] private RawImage displayImagePlayer;

    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Camera thirdPersonCameraPlayer;
    [SerializeField] private Camera firstPersonCamera;

    [SerializeField] private Transform backSpot;
    [SerializeField] private Transform frontSpot;

    [SerializeField] private GameObject loadingWarning;

    [Header("-----Settings-----")]

    [SerializeField] private KeyCode switchViewInput = KeyCode.X;


    [Header("-----Zoom Settings-----")]

    [SerializeField] private float zoomSpeed = 3.0f;
    [SerializeField] private float zoomMin = 1.0f;
    [SerializeField] private float zoomMax = 5.0f;

    [Header("-----Center Translate Settings-----")]

    [SerializeField] private KeyCode translateCenterInput = KeyCode.LeftControl;
    [SerializeField] private float deltaCenterSpeed = 1.0f;
    [SerializeField] private float maxDeltaCenter = 0.0f;
    [SerializeField] private float minDeltaCenter = -1.0f;

    [Header("-----Clip Settings-----")]

    [SerializeField] private LayerMask clipLayers;
    [SerializeField] private bool enableDeltaClip;
    [SerializeField] private float deltaClip = 0.1f;

    [Header("-----Player Fade-----")]

    [SerializeField] private bool enableGradientPlayer;
    [SerializeField] private float minimumPlayerDisplayDistance = 1.0f;
    [SerializeField] private float totalPlayerDisplayDistance = 2.0f;


    private RenderTexture firstPersonRenderTexture;
    private RenderTexture thirdPersonRenderTexture;
    private RenderTexture thirdPersonRenderTexturePlayer;
    private Vector2 displaySize;

    private int currentState; //0 = first, 1 = back, 2 = front
    private float currentFrontScroll = 1;
    private float currentBackScroll = 1;
    private float currentDeltaCenter = 0;


    private void Start()
    {
        if (Networking.LocalPlayer.IsUserInVR())
        {
            gameObject.SetActive(false);
            return;
        }

        currentState = 1;
        currentFrontScroll = (zoomMin + zoomMax) / 2;
        currentBackScroll = (zoomMin + zoomMax) / 2;

        UpdateRenderTexture();
    }

    private void UpdateRenderTexture()
    {
        Vector2 currentDisplaySize = new Vector2(displayImageTransform.rect.width, displayImageTransform.rect.height);

        if (currentDisplaySize.x == 0 && currentDisplaySize.y == 0) return;

        if (currentDisplaySize == displaySize && firstPersonRenderTexture != null && thirdPersonRenderTexture != null && thirdPersonRenderTexturePlayer != null) return;

        displaySize = currentDisplaySize;

        loadingWarning.SetActive(false);

        UpdateThirdRenderTexture();
        UpdateFirstRenderTexture();
       

        if (!enableGradientPlayer) return;

        UpdateThirdRenderTexturePlayer();
    }

    private void UpdateThirdRenderTexture()
    {
        if (thirdPersonRenderTexture != null)
        {
            thirdPersonRenderTexture.Release();
        }
        thirdPersonRenderTexture = new RenderTexture((int)displaySize.x, (int)displaySize.y, 32);
        thirdPersonRenderTexture.anisoLevel = 8;
        thirdPersonRenderTexture.autoGenerateMips = true;
        thirdPersonRenderTexture.antiAliasing = 8;

        thirdPersonCamera.targetTexture = thirdPersonRenderTexture;
        thirdPersonCamera.enabled = true;

        displayImage.texture = thirdPersonRenderTexture;
    }
    private void UpdateFirstRenderTexture()
    {
        if (firstPersonRenderTexture != null)
        {
            firstPersonRenderTexture.Release();
        }
        firstPersonRenderTexture = new RenderTexture((int)displaySize.x, (int)displaySize.y, 32);
        firstPersonRenderTexture.anisoLevel = 8;
        firstPersonRenderTexture.autoGenerateMips = true;
        firstPersonRenderTexture.antiAliasing = 8;

        firstPersonCamera.targetTexture = firstPersonRenderTexture;
        firstPersonCamera.enabled = true;

        displayImageUI.texture = firstPersonRenderTexture;
    }

    private void UpdateThirdRenderTexturePlayer()
    {
        if (thirdPersonRenderTexturePlayer != null)
        {
            thirdPersonRenderTexturePlayer.Release();
        }
        thirdPersonRenderTexturePlayer = new RenderTexture((int)displaySize.x, (int)displaySize.y, 32);
        thirdPersonRenderTexturePlayer.anisoLevel = 8;
        thirdPersonRenderTexturePlayer.autoGenerateMips = true;
        thirdPersonRenderTexturePlayer.antiAliasing = 8;

        thirdPersonCameraPlayer.targetTexture = thirdPersonRenderTexturePlayer;
        thirdPersonCameraPlayer.enabled = true;

        displayImagePlayer.texture = thirdPersonRenderTexturePlayer;
    }


    private void Update()
    {
        VRCPlayerApi.TrackingData trackingData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        transform.position = trackingData.position + currentDeltaCenter*Vector3.up;
        firstPersonCamera.transform.localPosition = currentDeltaCenter * Vector3.down;
        transform.rotation = trackingData.rotation;

        UpdateRenderTexture();


        if (Input.GetKeyDown(switchViewInput))
        {
            SwitchState();
        }


        UpdateScroll();
    }

    private void UpdateScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(translateCenterInput))
        {
            currentDeltaCenter += deltaCenterSpeed * scroll;
            currentDeltaCenter = Mathf.Clamp(currentDeltaCenter, minDeltaCenter, maxDeltaCenter);
            scroll = 0;
        }

        switch (currentState)
        {
            case 1:
                UpdateScrollBack(scroll);
                break;
            case 2:
                UpdateScrollFront(scroll);
                break;
        }
    }

    private void UpdateScrollBack(float scroll)
    {
        currentBackScroll -= scroll * zoomSpeed;
        currentBackScroll = Mathf.Clamp(currentBackScroll, zoomMin, zoomMax);
        backSpot.transform.localPosition = currentBackScroll * Vector3.back;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, backSpot.transform.position - transform.position, out hit, currentBackScroll + deltaClip, clipLayers, QueryTriggerInteraction.Ignore))
        {
            if ((hit.point - transform.position).sqrMagnitude > zoomMin * zoomMin)
            {
                backSpot.transform.position = hit.point;
            }
            else
            {
                backSpot.transform.localPosition = zoomMin * Vector3.back;
            }
            backSpot.transform.localPosition -= backSpot.transform.localPosition.normalized * deltaClip * (1.0f - Vector3.Dot(hit.normal, backSpot.transform.localPosition.normalized));

        }

        //Gradient

        float visibility = Mathf.Clamp01((backSpot.transform.localPosition.magnitude - minimumPlayerDisplayDistance) / (totalPlayerDisplayDistance - minimumPlayerDisplayDistance));

        if (visibility < 1 && enableGradientPlayer)
        {
            thirdPersonCamera.cullingMask &= ~((1 << 18) + (1 << 9) + (1 << 10)); //Disable the layer
            thirdPersonCameraPlayer.gameObject.SetActive(true);
            displayImagePlayer.color = new Color(1, 1, 1, visibility);
        }
        else
        {
            thirdPersonCamera.cullingMask |= ((1 << 18) + (1 << 9) + (1 << 10)); //Enable the layer
            displayImagePlayer.color = new Color(1, 1, 1, 0);
        }

    }

    private void UpdateScrollFront(float scroll)
    {
        currentFrontScroll -= scroll * zoomSpeed;
        currentFrontScroll = Mathf.Clamp(currentFrontScroll, zoomMin, zoomMax);
        frontSpot.transform.localPosition = currentFrontScroll * Vector3.forward;

        RaycastHit hit2;
        if (Physics.Raycast(transform.position, frontSpot.transform.position - transform.position, out hit2, currentFrontScroll + deltaClip))
        {
            if ((hit2.point - transform.position).sqrMagnitude > zoomMin * zoomMin)
            {
                frontSpot.transform.position = hit2.point;
            }
            else
            {
                frontSpot.transform.localPosition = zoomMin * Vector3.forward;
            }
            frontSpot.transform.localPosition -= frontSpot.transform.localPosition.normalized * deltaClip * (1.0f - Vector3.Dot(hit2.normal, frontSpot.transform.localPosition.normalized));

        }

        //Gradient

        float visibility2 = Mathf.Clamp01((frontSpot.transform.localPosition.magnitude - minimumPlayerDisplayDistance) / (totalPlayerDisplayDistance - minimumPlayerDisplayDistance));


        if (visibility2 < 1 && enableGradientPlayer)
        {
            thirdPersonCamera.cullingMask &= ~((1 << 18) + (1 << 9) + (1 << 10)); //Disable the layer
            thirdPersonCameraPlayer.gameObject.SetActive(true);
            displayImagePlayer.color = new Color(1, 1, 1, visibility2);
        }
        else
        {
            thirdPersonCamera.cullingMask |= ((1 << 18) + (1 << 9) + (1 << 10)); //Enable the layer
            displayImagePlayer.color = new Color(1, 1, 1, 0);
        }
    }

    private void SwitchState()
    {
        currentState = (currentState + 1) % 3;

        switch (currentState)
        {
            case 0:
                displayImage.gameObject.SetActive(false);
                displayImagePlayer.gameObject.SetActive(false);
                displayImageUI.gameObject.SetActive(false);
                break;
            case 1:
                Switch3rdPerson();
                thirdPersonCamera.transform.parent = backSpot;
                thirdPersonCamera.transform.localRotation = Quaternion.identity;
                thirdPersonCameraPlayer.transform.parent = backSpot;
                thirdPersonCameraPlayer.transform.localRotation = Quaternion.identity;
                thirdPersonCamera.transform.localPosition = Vector3.zero;
                thirdPersonCameraPlayer.transform.localPosition = Vector3.zero;
                break;
            case 2:
                Switch3rdPerson();
                thirdPersonCamera.transform.parent = frontSpot;
                thirdPersonCamera.transform.localRotation = Quaternion.Euler(0,180,0);
                thirdPersonCameraPlayer.transform.parent = frontSpot;
                thirdPersonCameraPlayer.transform.localRotation = Quaternion.Euler(0, 180, 0);
                thirdPersonCamera.transform.localPosition = Vector3.zero;
                thirdPersonCameraPlayer.transform.localPosition = Vector3.zero;
                break;
        }


    }

    private void Switch3rdPerson()
    {
        displayImage.gameObject.SetActive(true);
        displayImageUI.gameObject.SetActive(true);
        displayImagePlayer.gameObject.SetActive(true);
        
    }

}
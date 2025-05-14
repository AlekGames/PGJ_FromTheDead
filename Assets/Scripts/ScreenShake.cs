using Unity.Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance;

    private CinemachineCamera _cinemachineCamera;
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    private CinemachineBasicMultiChannelPerlin channel;

    private void Awake()
    {
        Instance = this;
        _cinemachineCamera = GetComponent<CinemachineCamera>();
    }

    private void Start()
    {
        channel = _cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensity, float time)
    {
        channel.AmplitudeGain = intensity;
        
        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                channel.AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1-(shakeTimer / shakeTimerTotal));
            }
        }
    }
}

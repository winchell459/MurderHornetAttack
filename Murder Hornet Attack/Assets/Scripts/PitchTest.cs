using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PitchTest : MonoBehaviour
{
    private Rigidbody2D Player;
    public float smoothing = 0.01f;
    public float Pitch = 1;
    public float MaxSpeed, MinSpeed;
    public float MaxPitch, MinPitch;
    
    public AudioSource AS;
    public enum TestingType
    {
        constant,
        continuous,
        dualstep,
        slowcontinuous,
        slowdualstep,
        averagecontinuous
    }
    public TestingType MyType;

    public Dropdown TestingTypeDropdown;
    public InputField inputMaxPitch, inputMinPitch, inputPitch, inputPitchRate;
    public Text textPitch;
    public Slider VolumeSlider;

    private float currentPitch;
    private float averageVelocity;

    // Start is called before the first frame update
    void Start()
    {
        AS = GetComponent<AudioSource>();
        setTestPanelParameters();
        currentPitch = Pitch;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Player) 
        {
            //Debug.Log(Player.velocity.magnitude);
            float pitch = AS.pitch;
            
            if(MyType == TestingType.continuous)
            {
                
                float percent = getSpeedPercent();
                pitch = percent * (MaxPitch - MinPitch) + MinPitch;
            }
            else if(MyType == TestingType.dualstep)
            {
                pitch = Player.velocity.magnitude < ((MaxSpeed - MinSpeed) / 2 + MinSpeed) ? MinPitch : MaxPitch;
            }
            else if (MyType == TestingType.slowcontinuous)
            {
               
                float pitchStep = (Player.velocity.magnitude < ((MaxSpeed - MinSpeed) / 2 + MinSpeed) ? -smoothing : smoothing)*Time.deltaTime;
                pitch += pitchStep;
            }
            else if(MyType == TestingType.slowdualstep)
            {
                float pitchStep = (Player.velocity.magnitude < ((MaxSpeed - MinSpeed) / 2 + MinSpeed) ? -smoothing : smoothing);//* Time.deltaTime;
                //Debug.Log("pitchStep: " + pitchStep );
                currentPitch += pitchStep * Time.deltaTime;
                currentPitch = Mathf.Clamp(currentPitch, MinPitch, MaxPitch);
                if (currentPitch >= MaxPitch) pitch = MaxPitch;
                else if (currentPitch <= MinPitch) pitch = MinPitch;
            }
            else if (MyType == TestingType.averagecontinuous)
            {
                float speed = Mathf.Clamp(Player.velocity.magnitude, MinSpeed, MaxSpeed);
                averageVelocity = (speed * smoothing + (1 - smoothing) * averageVelocity);
                float percent = getSpeedPercent(averageVelocity);
                pitch = percent * (MaxPitch - MinPitch) + MinPitch;
            }
            else
            {
                if (MaxPitch < Pitch) MaxPitch = Pitch;
                pitch = Pitch;
            }
            

            pitch = Mathf.Clamp(pitch, MinPitch, MaxPitch);
            AS.pitch = pitch;
            textPitch.text = pitch.ToString();
        }
        else
        {
            HornetController playerObj = FindObjectOfType<HornetController>();
            if(playerObj)
                Player = playerObj.GetComponent<Rigidbody2D>();
            
            
        }
    }
    void setTestPanelParameters()
    {
        string max = MaxPitch.ToString();
        string min = MinPitch.ToString();
        string pitch = Pitch.ToString();
        string pitchRate = smoothing.ToString();

        if(TestingTypeDropdown)TestingTypeDropdown.value = (int)MyType;
        if(inputMaxPitch)inputMaxPitch.text = max;
        if(inputMinPitch)inputMinPitch.text = min;
        if(inputPitch)inputPitch.text = pitch;
        if(inputPitchRate)inputPitchRate.text = pitchRate;
        if(VolumeSlider)VolumeSlider.value = 0.5f;
    }

    public void SetTestingType()
    {
        //MyType = type;
        //Debug.Log(TestingTypeDropdown.value);
        MyType = (TestingType)TestingTypeDropdown.value;
    }

    public void SetPitchParameters()
    {
        try
        {
            MaxPitch = float.Parse(inputMaxPitch.text);
            MinPitch = float.Parse(inputMinPitch.text);
            Pitch = float.Parse(inputPitch.text);
            smoothing = float.Parse(inputPitchRate.text);
            AS.volume = VolumeSlider.value;
        }
        catch
        {
            Debug.Log("Set Pitch Parameters Casting Error!");
        }
        
    }

    float getSpeedPercent()
    {
        float speed = Player.velocity.magnitude;
        return (speed - MinSpeed) / (MaxSpeed - MinSpeed);
    }

    float getSpeedPercent(float speed)
    {
        
        return (speed - MinSpeed) / (MaxSpeed - MinSpeed);
    }
}

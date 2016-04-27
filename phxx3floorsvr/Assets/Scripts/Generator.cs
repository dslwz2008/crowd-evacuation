using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Generator.
/// version : 0.5
/// </summary>
public class Generator : MonoBehaviour {
    public bool isPause = false;
	/// <summary>
	/// The charactors. user specified prefabs.
	/// </summary>
	public List<GameObject> charactors;
	
	/// <summary>
	/// The last frame data
	/// </summary>
	Frame lastFrame = null;

    GameObject parent = null;
    
    /// <summary>
	/// The time of one frame.
	/// </summary>
	public float frameTime = 0.1f;

    public AudioSource[] audioSources;

	private bool hasFinished = false;
	
	// Use this for initialization
	void Start () {
		parent = GameObject.Find("/Instances");
		InvokeRepeating("GenerateOneFrame", 1.5f, frameTime);
        LeanTween.init(800);
	}

    void Update() {
        if (Input.GetKeyDown("space")) {
            isPause = !isPause;
            if (isPause)
            {
                foreach (AudioSource audio in audioSources)
                {
                    audio.Pause();
                }
            }
            else
            {
                foreach (AudioSource audio in audioSources)
                {
                    audio.UnPause();
                }
            }
        }
    }
	
	void GenerateOneFrame(){
		if(isPause || hasFinished)
		{
			return;
		}

		Queue<Frame> queue = FrameBuffer.GetInstance().GetQueue();
		Frame thisFrame = null;
		if(queue.Count == 0){
			foreach(Transform people in parent.transform){
				if(people.gameObject.GetComponent<Animation>()){
					people.gameObject.GetComponent<Animation>().wrapMode = WrapMode.Loop;
					people.gameObject.GetComponent<Animation>().CrossFade("idle");
				}
			}
			//CancelInvoke("GenerateOneFrame");
            hasFinished = true;
            return;
		}else{
			thisFrame = queue.Dequeue();
		}
		//Debug.Log(thisFrame.id + "generating...");
		
		//this is the first frame
		if(lastFrame == null){
            System.Random random = new System.Random();
            foreach (int id in thisFrame.peoples.Keys)
            {
				int role = thisFrame.peoples[id].m_role;
				int chrctIndex = -1;//random.Next(charactors.Count);
				switch(role)
				{
				case 0://teacher
					chrctIndex = 15;
					break;
				case 1://student
					chrctIndex = 1;
					break;
				default://random role
					chrctIndex = random.Next(charactors.Count);
					break;
				}
				GameObject instance = (GameObject)Instantiate(
					charactors[chrctIndex],
					thisFrame.peoples[id].m_position, 
					Quaternion.Euler(0.0f, random.Next(90), 0.0f));

				instance.name = "people" + id.ToString();
				instance.transform.parent = parent.transform;
				instance.transform.localScale = Vector3.Scale(instance.transform.localScale, thisFrame.peoples[id].m_figureScale);
				if(instance.gameObject.GetComponent<Animation>()){
					instance.gameObject.GetComponent<Animation>().wrapMode = WrapMode.Loop;
					foreach(AnimationState state in instance.gameObject.GetComponent<Animation>()){
						state.speed = thisFrame.peoples[id].m_animSpeed;
					}
					instance.gameObject.GetComponent<Animation>().Play("idle");
				}
			}
            gameObject.SendMessage("InitialFinished", thisFrame.peoples.Count);
		}else{
            foreach (int id in thisFrame.peoples.Keys)
            {
                string name = "/Instances/people" + id.ToString();
                People people = thisFrame.peoples[id];

                GameObject goPeople = GameObject.Find(name);
                Vector3 pos = people.m_position;
                PeopleStatus status = people.m_status;
                //goPeople.transform.position = pos;
                LeanTween.move(goPeople, pos, frameTime).setEase(LeanTweenType.linear);
                goPeople.transform.forward = people.m_orientation;

                if (status == PeopleStatus.WALK)
                {//walk
                    //play animation
                    if (goPeople.gameObject.GetComponent<Animation>())
                    {
                        goPeople.gameObject.GetComponent<Animation>().wrapMode = WrapMode.Loop;
                        goPeople.gameObject.GetComponent<Animation>().CrossFade("walk");
                    }
                }
                else if (status == PeopleStatus.RUN)
                {//run
                    //play animation
                    if (goPeople.gameObject.GetComponent<Animation>())
                    {
                        goPeople.gameObject.GetComponent<Animation>().wrapMode = WrapMode.Loop;
                        goPeople.gameObject.GetComponent<Animation>().CrossFade("run");
                    }
                }
                else if (status == PeopleStatus.IDLE || status == PeopleStatus.STAND)
                {//idle, on the escalator
                    //play animation
                    if (goPeople.gameObject.GetComponent<Animation>())
                    {
                        goPeople.gameObject.GetComponent<Animation>().wrapMode = WrapMode.Loop;
                        goPeople.gameObject.GetComponent<Animation>().CrossFade("idle");
                    }
                }
            }
		}
		lastFrame = thisFrame;
	}

    IEnumerator ReStart()
    {
        yield return new WaitForSeconds(2);
        hasFinished = false;
        Debug.Log("restart!");
    }
}

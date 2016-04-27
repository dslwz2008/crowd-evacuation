using UnityEngine;
using System.Collections;

public class VRCameraFollow : MonoBehaviour {
    GameObject person = null;
    GameObject vrCamera = null;
    bool initialized = false;
    float cameraOffsetY = 1.0f;
    float cameraOffsetX = 0.1f;
    float cameraOffsetZ = 0f;

	// Use this for initialization
	void Start () {
        vrCamera = GameObject.Find("OVRCameraRig");
	}

    void InitialFinished(int peopleCount)
    {
        int personId = 1;
        string instanceName = "people" + personId.ToString();
        person = GameObject.Find("/Instances/" + instanceName);
        initialized = true;
        //这样的话，相机会自动跟随角色转动
        //vrCamera.transform.parent = person.transform;
        //vrCamera.transform.localPosition = new Vector3(0f, 1.8f, 0f);
    }

    void Update() {
        //高低
        if (Input.GetKeyDown(KeyCode.W))
        {
            cameraOffsetY += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            cameraOffsetY -= 0.1f;
        }
        //左右
        if (Input.GetKeyDown(KeyCode.A))
        {
            cameraOffsetX += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            cameraOffsetX -= 0.1f;
        }
        //前后
        if (Input.GetKeyDown(KeyCode.Z))
        {
            cameraOffsetZ += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            cameraOffsetZ -= 0.1f;
        }
    }

    void LateUpdate() {
        if (!initialized)
        {
            return;
        }
        vrCamera.transform.position = person.transform.position + new Vector3(cameraOffsetX, cameraOffsetY, cameraOffsetZ);
    }
}

using UnityEngine;

public class CameraControll : MonoBehaviour
{
	public GameObject TargetObj;
	private Vector3 previousTargetPos;

	void Start()
    {
        previousTargetPos = TargetObj.transform.position;
	}

    void Update()
    {
        Vector3 targetPosDiff = TargetObj.transform.position - previousTargetPos;
		transform.position = transform.position + targetPosDiff;
		previousTargetPos = TargetObj.transform.position;
        
        if (Input.GetMouseButton(0))
        {
			float mouseInputX = Input.GetAxis("Mouse X");
			transform.RotateAround(TargetObj.transform.position, Vector3.up, mouseInputX * Time.deltaTime * 180f);
		}
    }
}

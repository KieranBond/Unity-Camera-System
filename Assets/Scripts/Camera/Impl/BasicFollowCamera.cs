using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFollowCamera : MonoBehaviour
{
    [SerializeField]
    private Transform m_target;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tPos = m_target.position;
        transform.position = new Vector3(tPos.x, tPos.y, transform.position.z);
    }
}

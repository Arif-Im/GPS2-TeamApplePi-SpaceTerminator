using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : MonoBehaviour
{
    public Grenade shellPrefab;
    public GameObject shellSpawnPos;
    public GameObject target;
    float turnSpeed = 2;
    public GameObject parent;
    float speed = 15;
    bool canShoot = true;
    public bool activate;

    // Start is called before the first frame update
    void Start()
    {

    }

    void CanShootAgain()
    {
        canShoot = true;
        activate = false;
    }

    public void Fire()
    {
        if (canShoot)
        {
            Debug.Log("Shoot");
            Grenade shell = Instantiate(shellPrefab, shellSpawnPos.transform.position, shellSpawnPos.transform.rotation);
            shell.GetComponent<Rigidbody>().velocity = speed * this.transform.forward;
            canShoot = false;
            Invoke("CanShootAgain", 0.5f);
        }
    }

    public void Activate()
    {
        activate = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            activate = true;
        }
        Vector3 direction = (target.transform.position - parent.transform.position).normalized;
        if(activate)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            parent.transform.rotation = Quaternion.Slerp(parent.transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }

        float? angle = RotateTurret();

        if (angle != null && Vector3.Angle(direction, parent.transform.forward) < 1 && activate)
        {
            Fire();
        }
    }


    float? RotateTurret()
    {
        float? angle = CalculateAngle(false);
        if (angle != null)
        {
            this.transform.localEulerAngles = new Vector3(360f - (float)angle, 0f, 0f);
        }
        return angle;
    }
    float? CalculateAngle(bool low) //either returns a float or a null value
    {
        Vector3 targetDir = target.transform.position - this.transform.position; //direction of player to target
        float y = targetDir.y; // y of direction of player to target
        targetDir.y = 0f; // set y of direction to 0
        float x = targetDir.magnitude; // set x to the magnitude of the direction
        float gravity = 9.81f; // set gravity of earth
        float speedSqr = speed * speed; // set square of speed
        float underTheSqrRoot = (speedSqr * speedSqr) - gravity * (gravity * x * x + 2 * y * speedSqr);

        if (underTheSqrRoot >= 0f)
        {
            float root = Mathf.Sqrt(underTheSqrRoot);
            float highAngle = speedSqr + root;
            float lowAngle = speedSqr - root;

            if (low)
                return (Mathf.Atan2(lowAngle, gravity * x) * Mathf.Rad2Deg);
            else
                return (Mathf.Atan2(highAngle, gravity * x) * Mathf.Rad2Deg);
        }
        else
            return null;
    }
}

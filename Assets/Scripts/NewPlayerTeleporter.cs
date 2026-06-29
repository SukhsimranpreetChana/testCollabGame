using UnityEngine;

public class NewPlayerTeleporter : MonoBehaviour
{
    public Transform TeleportZoneObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 localOffest = transform.InverseTransformPoint(other.transform.position);

            Quaternion relativeRotation = TeleportZoneObject.rotation * Quaternion.Inverse(transform.rotation);
            
            CharacterController cc = other.GetComponent<CharacterController>();

            if(cc != null)
            {
                cc.enabled = false;

                other.transform.position = TeleportZoneObject.TransformPoint(localOffest);

                other.transform.rotation = relativeRotation * other.transform.rotation;

                cc.enabled = true;
            }
        }
    }
}

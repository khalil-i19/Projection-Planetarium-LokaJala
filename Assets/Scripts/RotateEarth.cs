using UnityEngine;

public class RotateEarth : MonoBehaviour
{
    // Derajat rotasi bola (0-360)
    [Range(0.0f, 360.0f)]
    public float currentDegrees = 0.0f;

    // Kemiringan Bumi dalam derajat
    [Range(0.0f, 100.0f)]
    public float axisTilt = 23.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Atur rotasi awal jika diperlukan
        RotateToCurrentDegrees();
    }

    // Update is called once per frame
    void Update()
    {
        // Atur rotasi berdasarkan derajat yang diatur
        RotateToCurrentDegrees();
    }

    // Metode untuk mengatur rotasi bola berdasarkan currentDegrees
    void RotateToCurrentDegrees()
    {
        // Menggunakan sumbu rotasi sesuai dengan kemiringan Bumi (sumbu X)
        Vector3 rotationAxis = new Vector3(1, 0, 0); // Rotasi mengelilingi sumbu X
        transform.rotation = Quaternion.AngleAxis(currentDegrees, rotationAxis);
    }
}

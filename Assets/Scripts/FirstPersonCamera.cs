using UnityEngine;

public class FirstPersonCamera : MonoBehaviour {

    public Transform characterBody;
    public Transform characterHead;

    float sensitivityX;
    float sensitivityY;

    float rotationX = 0;
    float rotationY = 0;

    float angleYmin = -90;
    float angleYmax = 90;

    [HideInInspector] public float nauseaTilt = 0f;

    void Start() {

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        float sensibilidadeSalva = PlayerPrefs.GetFloat("Sensibilidade", 1.0f);
        
        sensitivityX = sensibilidadeSalva;
        sensitivityY = sensibilidadeSalva;
    }

    private void LateUpdate() {
        transform.position = characterHead.position;
    }

    void Update() {
        if (Time.timeScale == 0f) return;

        // Pega a movimentação do mouse multiplicada pela sensibilidade
        float verticalDelta = Input.GetAxisRaw("Mouse Y") * sensitivityY;
        float horizontalDelta = Input.GetAxisRaw("Mouse X") * sensitivityX;

        rotationX += horizontalDelta;
        rotationY += verticalDelta;

        // Limita a rotação vertical para a câmera não dar um giro de 360 graus
        rotationY = Mathf.Clamp(rotationY, angleYmin, angleYmax);

        // Aplica a rotação no corpo e na câmera
        characterBody.localEulerAngles = new Vector3(0, rotationX, 0);
        transform.localEulerAngles = new Vector3(-rotationY, rotationX, nauseaTilt);
    }
}
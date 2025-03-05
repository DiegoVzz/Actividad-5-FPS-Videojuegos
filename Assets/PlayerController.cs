using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Cámara")]
    public Transform cameraTransform;
    public float mouseSensitivity = 100f;
    public float maxVerticalAngle = 80f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    [Header("Disparo")]
    public float fireRate = 0.2f;
    public float weaponDamage = 10f;
    public float bulletSpread = 0.1f;
    public float maxDistance = 100f;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private float nextFireTime;

    public GameObject bulletTrailPrefab;
    public float bulletTrailDuration = 0.1f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        #if UNITY_EDITOR
        UnityEditor.EditorWindow.focusedWindow.SendEvent(new Event { type = EventType.MouseDown });
        #endif
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();

        // Lógica de disparo
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        // Efecto de disparo
        if (muzzleFlash != null) muzzleFlash.Play();

        // Cálculo de la dirección con spread
        Vector3 shootDirection = cameraTransform.forward;
        shootDirection += new Vector3(
            Random.Range(-bulletSpread, bulletSpread),
            Random.Range(-bulletSpread, bulletSpread),
            Random.Range(-bulletSpread, bulletSpread)
        );
        shootDirection.Normalize();

        // Declara hit fuera del if
        RaycastHit hit;
        bool hasHit = Physics.Raycast(cameraTransform.position, shootDirection, out hit, maxDistance);

        // Sistema de trazo de bala
        GameObject bulletTrail = Instantiate(bulletTrailPrefab, muzzleFlash.transform.position, Quaternion.identity);
        LineRenderer line = bulletTrail.GetComponent<LineRenderer>();

        if (line != null)
        {
            line.SetPosition(0, muzzleFlash.transform.position);
            // Usa hit.point si hay impacto, si no, usa distancia máxima
            Vector3 endPosition = hasHit ? hit.point : cameraTransform.position + shootDirection * maxDistance;
            line.SetPosition(1, endPosition);
            Destroy(bulletTrail, bulletTrailDuration);
        }

        // Lógica de impacto
        if (hasHit)
        {
            // Daño a enemigos
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(); // Reduce 1 vida
            }

            // Daño a otros objetos (opcional)
            TargetScript target = hit.collider.GetComponent<TargetScript>();
            if (target != null)
            {
                target.TakeDamage(weaponDamage);
            }

            // Efecto de impacto
            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }

        // Debug
        Debug.DrawRay(cameraTransform.position, shootDirection * maxDistance, Color.red, 1f);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 0.1f; // Eliminamos Time.deltaTime
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 0.1f;

        // Rotación vertical (arriba/abajo)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxVerticalAngle, maxVerticalAngle);
        
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Rotación horizontal (izquierda/derecha)
        transform.Rotate(Vector3.up * mouseX);

        cameraTransform.localRotation = Quaternion.Lerp(
            cameraTransform.localRotation,
            Quaternion.Euler(xRotation, 0f, 0f),
            Time.deltaTime * 15f
        );

        // Asegúrate que solo el jugador rota horizontalmente
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(0f, transform.eulerAngles.y + mouseX, 0f),
            Time.deltaTime * 15f
        );
    }

    void HandleMovement()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

}
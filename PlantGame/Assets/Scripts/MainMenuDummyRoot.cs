using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuDummyRoot : MonoBehaviour {
    [HideInInspector] public float generatePointTimer = 0f;
    [HideInInspector] public float speed = 600f;

    [HideInInspector] public Vector2 target;

    public SpriteRenderer currentPointer;
    public Rigidbody2D rb;
    [HideInInspector] public LineRenderer stem;
    Quaternion lastRotation;
    [HideInInspector] public bool currentlyRotating;

    public Camera defaultCamera;

    private void Awake() {
        stem = GetComponent<LineRenderer>();
        stem.SetPosition(0, new Vector3(transform.position.x, transform.position.y + 1000, 0f));
        stem.SetPosition(1, transform.position);

        target = transform.position + (transform.right * speed * Time.deltaTime);

        UpdateColor(Color.green);
    }

    // Update is called once per frame
    void Update() {
        defaultCamera.transform.position = Vector3.Lerp(defaultCamera.transform.position, new Vector3(transform.position.x, transform.position.y, -10f), Time.deltaTime * 16f);
        target = defaultCamera.ScreenToWorldPoint(Input.mousePosition);

        if (currentlyRotating) {
            // Because we are changing angles, add an extra point to our linerenderer 10 times a second
            // This would be for saving roots and minimizing linerender calculations for performance purposes
            // The longer the timer, the more rigid the line appears but the healthier performance will be
            generatePointTimer = Mathf.Max(0f, generatePointTimer - Time.deltaTime);
            if (generatePointTimer == 0f) {
                stem.positionCount++;
                generatePointTimer = 0.1f;
            }
        } else generatePointTimer = 0f;

        stem.SetPosition(stem.positionCount - 1, transform.position);
    }

    private void FixedUpdate() {
        float angle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        if (lastRotation != targetRotation) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 400f * Time.deltaTime);
            lastRotation = transform.rotation;
            currentlyRotating = true;
        } else {
            currentlyRotating = false;
        }

        rb.velocity = transform.right * speed * Time.deltaTime;
    }

    void UpdateColor(Color c) {
        currentPointer.color = c;
        stem.endColor = c;
    }
}

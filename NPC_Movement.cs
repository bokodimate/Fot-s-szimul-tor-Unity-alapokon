using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Movement : MonoBehaviour {
    private int positionNumber = 1;
    Vector3 theDestination;
    bool lookRotationSet = false;
    public Animator anim;
    string currentAnimation;

    System.Random rnd = new System.Random();


    Color meshColor = Color.red;
    Mesh mesh;

    public float speed = 2f;
    // Start is called before the first frame update
    void Start() {
        theDestination = new Vector3(754, 14.545f, 274.4f);
        StartCoroutine(NextLocation());
    }

    // Update is called once per frame
    void Update() {
        if (!lookRotationSet) {
            lookRotationSet = true;
            transform.LookAt(theDestination);
        }

        if (Math.Floor(transform.position.x) == Math.Floor(theDestination.x) && Math.Floor(transform.position.z) == Math.Floor(theDestination.z)) {

            currentAnimation = "idle";
            try {
                anim.Play(currentAnimation);
            } catch (Exception) {

            }

            lookRotationSet = false;
        }

        var step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, theDestination, step);
    }

    IEnumerator NextLocation() {
        if (positionNumber == 1) { // Kút elõtt
            yield return new WaitForSeconds(60);

            currentAnimation = "walk";
            anim.Play(currentAnimation);

            theDestination = new Vector3(754,14.545f, 274.4f);

            positionNumber = 2;
            speed = 2f;

            StartCoroutine(NextLocation());

        }
        if (positionNumber == 2) { // Templom lépcsõ elõtt
            yield return new WaitForSeconds(5);

            currentAnimation = "walk";
            anim.Play(currentAnimation);

            theDestination = new Vector3(755.17f, 14.65f, 262.377f);

            positionNumber = 3;
            speed = 2f;

            StartCoroutine(NextLocation());
        }
        if (positionNumber == 3) { // Templom lépcsõ tetején
            yield return new WaitForSeconds(5);

            currentAnimation = "walk";
            anim.Play(currentAnimation);

            theDestination = new Vector3(755.17f, 15.374f, 259.738f);

            positionNumber = 4;
            speed = 2f;

            StartCoroutine(NextLocation());
        }
        if (positionNumber == 4) { // Templomban
            yield return new WaitForSeconds(5);

            currentAnimation = "walk";
            anim.Play(currentAnimation);

            theDestination = new Vector3(755.17f, 15.374f, 245.91f);

            positionNumber = 5;
            speed = 2f;

            StartCoroutine(NextLocation());
        }
        if (positionNumber == 5) { // Templom lépcsõ tetején
            yield return new WaitForSeconds(30);

            currentAnimation = "walk";
            anim.Play(currentAnimation);

            theDestination = new Vector3(755.17f, 15.374f, 259.738f);

            positionNumber = 6;
            speed = 2f;

            StartCoroutine(NextLocation());
        }
        if (positionNumber == 6) { // Templom lépcsõ elõtt
            yield return new WaitForSeconds(5);

            currentAnimation = "walk";
            anim.Play(currentAnimation);

            theDestination = new Vector3(755.17f, 14.65f, 262.377f);

            positionNumber = 7;
            speed = 2f;

            StartCoroutine(NextLocation());
        }
        if (positionNumber == 7) { // Ház sarkánál
            yield return new WaitForSeconds(5);

            currentAnimation = "walk";
            anim.Play(currentAnimation);

            theDestination = new Vector3(769.18f, 14.696f, 277.15f);

            positionNumber = 8;
            speed = 2f;

            StartCoroutine(NextLocation());
        }
        if (positionNumber == 8) { // Autónál
            yield return new WaitForSeconds(60);

            currentAnimation = "walk";
            anim.Play(currentAnimation);

            theDestination = new Vector3(801.483f, 15.803f, 284.426f);

            positionNumber = 1;
            speed = 2f;

            StartCoroutine(NextLocation());
        }

    }
}

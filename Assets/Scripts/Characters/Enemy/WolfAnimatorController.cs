using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfAnimatorController : MonoBehaviour {
    private int counter = 0;
	// Update is called once per frame
	void FixedUpdate () {
        counter++;
        if (counter == 12) {
            counter = 0;
            int random = Random.Range(0, 100);
            if (random <= 5 &&
            !this.GetComponent<Animator>().GetBool("shouldTriggerUpDown"))
            {
                this.GetComponent<Animator>().SetBool("shouldTriggerUpDown", true);
            }
            else if ((random > 10 && random < 20) &&
            !this.GetComponent<Animator>().GetBool("shouldTriggerOpenMouth")) {
                this.GetComponent<Animator>().SetBool("shouldTriggerOpenMouth", true);
            }

        } 
	}

    void finishedAnimation() {
        this.GetComponent<Animator>().SetBool("shouldTriggerUpDown", false);
        this.GetComponent<Animator>().SetBool("shouldTriggerOpenMouth", false);
    }
   
}

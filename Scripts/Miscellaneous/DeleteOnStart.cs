using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOnStart : MonoBehaviour
{
    private void Start()
    {
        Destroy(this.gameObject);
    }
}

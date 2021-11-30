using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCardEffect : MonoBehaviour
{
    [SerializeField] private List<FieldEffect> keys;
    [SerializeField] private List<string> values;

    public List<FieldEffect> Keys => keys;

    public List<string> Values => values;
}

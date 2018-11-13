using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathStatistics {

	public static float GenerateStandardNormalNumber (float mean, float standardDeviation) {

		float u1 = Random.value;
		float u2 = Random.value;

		float randomStandardNormal = Mathf.Sqrt (-2.0f * Mathf.Log (u1)) * Mathf.Sin (2.0f * Mathf.PI * u2);
		float randomNormal = mean + standardDeviation * randomStandardNormal;

		return randomNormal;
	}
}

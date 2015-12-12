//========================================================================================================================
// Edy Vehicle Physics - (c) Angel Garcia "Edy" - Oviedo, Spain
// http://www.edy.es/dev/vehicle-physics
// 
// Terms & Conditions:
//  - Use for unlimited time, any number of projects, royalty-free.
//  - Keep the copyright notices on top of the source files.
//  - Resale or redistribute as anything except a final product to the end user (asset / library / engine / middleware / etc.) is not allowed.
//  - Put me (Angel Garcia "Edy") in your game's credits as author of the vehicle physics.
//
// Bug reports, improvements to the code, suggestions on further developments, etc are always welcome.
// Unity forum user: Edy
//========================================================================================================================
//
// CarWheelFriction
//
// Magic. Do not touch.
//
//========================================================================================================================
using UnityEngine;
using System.Collections;

public class CarWheelFriction : ScriptableObject
{

	// Slope:	Número que denota una pendiente en extremumValue o asymptoteValue de WheelFrictionCurve
	//			Las "Magic Curve" relacionan el número Slope con la pendiente de la recta resultante en la gráfica WheelFrictionCurve.
	// Value:	Valor de aceleración (m/s2, fuerza por masa) producido por la curva WheelFrictionCurve (eje Y)
	// Slip: 	valor de desplazamiento (m/s) usado como entrada en la curva WheelFrictionCurve (eje X)
	
	
	// ============= WheelCollider's MAGIC CURVES ==================
	// Coeficientes de las magic curves.
	// Son los valores de fricción (Value) en Slip 1.0 para pendientes (Slopes) de 50 en 50, desde 50 a 2500 (total: 50 coeficientes)

	public static float[] MCforward = new float[] {0.561f, 1.114f, 1.659f, 2.195f, 2.724f, 3.246f, 3.759f, 4.266f, 4.765f, 5.257f, 5.741f, 6.219f, 6.690f, 7.154f, 7.611f, 8.062f, 8.506f, 8.944f, 9.376f, 9.801f, 10.223f, 10.636f, 11.044f, 11.443f, 11.839f, 12.229f, 12.614f, 12.990f, 13.363f, 13.731f, 14.095f, 14.452f, 14.805f, 15.153f, 15.496f, 15.834f, 16.167f, 16.495f, 16.819f, 17.138f, 17.453f, 17.772f, 18.078f, 18.380f, 18.667f, 18.970f, 19.260f, 19.545f, 19.826f, 20.103f };
	public static float[] MCsideways = new float[] { 0.555f, 1.095f, 1.622f, 2.135f, 2.636f, 3.124f, 3.600f, 4.060f, 4.513f, 4.951f, 5.382f, 5.793f, 6.199f, 6.602f, 7.002f, 7.348f, 7.723f, 8.096f, 8.419f, 8.782f,  9.101f,  9.403f,  9.779f, 10.026f, 10.322f, 10.695f, 10.921f, 11.157f, 11.465f, 11.807f, 12.005f, 12.202f, 12.440f, 12.741f, 13.060f, 13.234f, 13.394f, 13.570f, 13.780f, 14.033f, 14.333f, 14.567f, 14.707f, 14.835f, 14.969f, 15.119f, 15.291f, 15.489f, 15.714f, 15.966f };
	//                          50    100    150    200    250    300    350    400    450    500    550    600    650    700    750    800    850    900    950   1000    1050    1100    1150    1200    1250    1300    1350    1400    1450    1500    1550    1600    1650    1700    1750    1800    1850    1900    1950    2000    2050    2100    2150    2200    2250    2300    2350    2400    2450    2500

	// Factores de ajuste para predecir los valores y representar las curvas de fricción

	public static float MCt0 = 0.9f;
	public static float MCt1 = 1.3f; 

	// =============================================================


	// Dada una curva de fricción (grip, gripRange, drift) determinar el valor máximo y su posición.
	// 
	// El valor se usa para calibrar la curva con la aceleración máxima del vehículo (peakValue), 
	// y la posición se usa para determinar el estado de agarre de la rueda (peakSlip).

	public static Vector2 GetPeakValue (float[] Coefs, float grip, float gripRange, float drift)
	{
		return GetSpLineMax (1.0f, GetValue (Coefs, grip, 1.0f), 1.0f + gripRange, GetValue (Coefs, drift, 1.0f + gripRange));
	}

	
	// Dado un punto en la gráfica en forma de (Slip, Value) calcular el valor Slope que utilizado
	// en grip o drift haría pasar la recta por ese punto.
	// 
	// Se usa para corregir la curva del wheelcollider.

	public static float GetSlope (float[] Coefs, float Slip, float Value)
	{
		// Determinar la pendiente de la linea, o lo que es lo mismo, el valor que tiene en Slip = 1.0
	
		float lineSlope = Value / Slip;
	
		// Descartar límites
	
		if (lineSlope <= Coefs [0])
			return Mathf.InverseLerp (0.0f, Coefs [0], lineSlope) * 50;
		else if (lineSlope >= Coefs [Coefs.Length - 1])
			return Coefs.Length * 50;
		
		// Interpolar valor en la lista. Localizamos el intervalo por búsqueda binaria.

		int left = -1;
		int right = Coefs.Length;
	
		while (right-left > 1) {
			int mid = (left + right) >> 1;
		
			if (Coefs [mid] < lineSlope)
				left = mid;
			else
				right = mid;
		}
		
		// Como ya hemos comprobado los límites, el valor buscado está en el intervalo (left, right]

		float x0 = Coefs [left];
		float x1 = Coefs [right];
		float y0 = (left + 1) * 50;
		float y1 = (right + 1) * 50;
	
		float Slope = y0 + (y1 - y0) / (x1 - x0) * (lineSlope - x0);
		return Slope;
	}


	// Dado un valor de slope de WheelFrictionCurve (extremumValue, asymptoteValue) y un punto de Slip
	// devuelve el valor (aceleración) que daría esa Slope en el punto de Slip dado.
	//
	// Se usa para obtener las pendientes de la curva en los puntos extremum y asymptote,
	// y calcular con ello las tangentes de la spline.

	public static float GetValue (float[] Coefs, float Slope, float Slip)
	{
		float lineSlope;
		float x0;
		float y0;
		float x1;
		float y1;
		int i;
	
		// Obtener la pendiente real (Value) de la recta Slope

		if (Slope <= 50.0f)
			lineSlope = Coefs [0] * Slope / 50.0f;
		else if (Slope >= Coefs.Length * 50.0f)
			lineSlope = Coefs [Coefs.Length - 1];
		else {
			// 50 < Slope < Coefs.length*50
		
			i = (int) (Slope / (float) 50);
		
			x0 = i * 50;
			x1 = (i + 1) * 50;

			y0 = Coefs [i - 1];
			y1 = Coefs [i];
		
			lineSlope = y0 + (y1 - y0) / (x1 - x0) * (Slope - x0);
		}
	
		return lineSlope * Slip;	
	}


	// Dada la curva de fricción entre los dos puntos dados determinar el punto con el valor máximo.
	//
	// Ojo, se asume la forma típica de la curva de fricción entre extremum y asymptote.
	// si la curva resulta siempre ascendente el valor extremum coincidirá con el asymptote.

	private static Vector2 GetSpLineMax (float Slip0, float Value0, float Slip1, float Value1)
	{
		float s;
		float s2;
		float s3;
		float h1;
		float h2;
		float h3;
		float h4;
		Vector2 P;
	
		Vector2 PMax = new Vector2 (0, -Mathf.Infinity);

		// Definir los puntos y las tangentes
	
		Vector2 P0 = new Vector2 (Slip0, Value0);
		Vector2 P1 = new Vector2 (Slip1, Value1);
		float sl0 = Value0 / Slip0;
		//float sl1 = Value1 / Slip1;

		float T0mult = (1 + (Slip1 - Slip0) / (Slip1 - Slip0 + 1)) * MCt0;
		Vector2 T0 = new Vector2 (Slip1 - Slip0, sl0 * Slip1 - Value0) * T0mult;
		Vector2 T1 = new Vector2 (Slip1 - Slip0, 0) * MCt1;
	
		// Iterar la spline localizando el punto máximo.
	
		int Steps = (int) ((Slip1 - Slip0) * 10);
		if (Steps < 10)
			Steps = 10;

		for (int t=0; t<=Steps; t++) {
			s = t;
			s /= Steps;
			s2 = s * s;
			s3 = s2 * s;
		
			// Valores de las funciones de Hermite
		
			h1 = 2 * s3 - 3 * s2 + 1;
			h2 = -2 * s3 + 3 * s2;
			h3 = s3 - 2 * s2 + s;
			h4 = s3 - s2;
		
			// Punto interpolado
		
			P = h1 * P0 + h2 * P1 + h3 * T0 + h4 * T1;
				
			// Una curva de fricción empieza siempre ascendente hacia el máximo, luego baja, y puede volver a crecer antes de terminar.
		
			if (P.y >= PMax.y)
				PMax = P;
			else
				break;
		}

		return PMax;
	}
	

	// Dibujar la curva de fricción de una rueda en el canvas dado
	
	public static void DrawFrictionCurve (GUICanvas Canvas, float[] Coefs, CarFrictionCurve curve, Color col)
	{	
		float grip = curve.grip;
		float drift = curve.drift;
	
		if (grip < drift)
			grip = drift;
	
		float Slip0 = 1.0f;
		float Value0 = GetValue (Coefs, grip, Slip0);
		float Slip1 = 1.0f + curve.gripRange;
		float Value1 = GetValue (Coefs, drift, Slip1);
	
		Canvas.FrictionCurve (Slip0, Value0, Slip1, Value1, curve.driftSlope, col);
	}
	
	
	// Dibujar la curva de fricción dada aplicando un escalado de slip dado
	
	public static void DrawScaledFrictionCurve (GUICanvas Canvas, float[] Coefs, CarFrictionCurve curve, float scale, Color col)
	{	
		float grip = curve.grip;
		float drift = curve.drift;
	
		if (grip < drift)
			grip = drift;
	
		float Slip0 = 1.0f * scale;
		float Value0 = GetValue (Coefs, grip, Slip0);
		float Slip1 = (1.0f + curve.gripRange) * scale;
		float Value1 = GetValue (Coefs, drift, Slip1);
	
		Canvas.FrictionCurve (Slip0, Value0, Slip1, Value1, curve.driftSlope, col);
	}
	

}

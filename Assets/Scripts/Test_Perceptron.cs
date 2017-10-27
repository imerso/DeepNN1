//
// Written by Vander Roberto Nunes Dias, a.k.a. "imerso" / imersiva.com
//
// Simples Tests for Perceptron Neurons
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Perceptron : MonoBehaviour
{
	void Start()
	{
		//
		// NAND port using a Perceptron
		//

		Neuron_Perceptron neuronNAND;
		neuronNAND = new Neuron_Perceptron(3);
		neuronNAND.inputs.Add(new NeuronDentrite(neuronNAND, -2, 1));
		neuronNAND.inputs.Add(new NeuronDentrite(neuronNAND, -2, 1));

		// 00, 01, 10 and 11 binary inputs
		for (int i = 0; i < 4; i++)
		{
			int i1 = i & 1;
			int i2 = (i >> 1) & 1;
			neuronNAND.inputs[0].value = i1;
			neuronNAND.inputs[1].value = i2;
			neuronNAND.Update();
			Debug.Log("NAND " + i2 + "," + i1 + " = " + neuronNAND.output.value);
		}

		//
		// 2 bits binary sum circuit using Perceptrons
		//

		Neuron_Perceptron[] neuronSUM = new Neuron_Perceptron[5];
		for (int i = 0; i < 5; i++)
		{
			neuronSUM[i] = new Neuron_Perceptron(3);
			neuronSUM[i].inputs.Add(new NeuronDentrite(neuronSUM[i], -2, 0));
			neuronSUM[i].inputs.Add(new NeuronDentrite(neuronSUM[i], -2, 0));
		}

		neuronSUM[1].inputs[0].synapses.Add(neuronSUM[0].inputs[0]);
		neuronSUM[1].inputs[1].synapses.Add(neuronSUM[0].output);
		neuronSUM[2].inputs[0].synapses.Add(neuronSUM[0].output);
		neuronSUM[2].inputs[1].synapses.Add(neuronSUM[0].inputs[1]);
		neuronSUM[3].inputs[0].synapses.Add(neuronSUM[0].output);
		neuronSUM[3].inputs[1].synapses.Add(neuronSUM[0].output);
		neuronSUM[4].inputs[0].synapses.Add(neuronSUM[1].output);
		neuronSUM[4].inputs[1].synapses.Add(neuronSUM[2].output);

		for (int i = 0; i < 4; i++)
		{
			int i1 = i & 1;
			int i2 = (i >> 1) & 1;

			neuronSUM[0].inputs[0].value = i1;
			neuronSUM[0].inputs[1].value = i2;

			for (int j = 0; j < neuronSUM.Length; j++)
			{
				neuronSUM[j].Update();
			}

			Debug.Log("Binary Sum " + i1 + "+" + i2 + " = " + neuronSUM[3].output.value + "" + neuronSUM[4].output.value);
		}
	}
}

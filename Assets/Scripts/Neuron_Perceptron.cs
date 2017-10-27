//
// Written by Vander Roberto Nunes Dias, a.k.a. "imerso" / imersiva.com
//
// Perceptron Neuron class
//

using System.Collections;
using System.Collections.Generic;

public class Neuron_Perceptron : Neuron
{
	// Perceptron constructor
	public Neuron_Perceptron(float bias) : base(bias)
	{
	}

	// update the neuron,
	// by processing its inputs, with a simple weighted
	// sum of all its inputs, then finally generating the output.
	public override void Update()
	{
		float weightedSum = 0;

		for (int i = 0; i < inputs.Count; i++)
		{
			// only updates the weighted input if it has synapses,
			// else keep the original value set by user.
			if (inputs[i].synapses.Count > 0)
			{
				// this input has synapses, which may or may not
				// be an output from another neuron
				for (int j = 0; j < inputs[i].synapses.Count; j++)
				{
					// we only use the value from the synapse
					inputs[i].value = inputs[i].synapses[j].value;
					// and weight the input by its own weight
					weightedSum += inputs[i].value * inputs[i].weight;
				}
			}
			else
			{
				// this input has no synapses, it is a first-layer input,
				// so we use its direct value (set by user)
				weightedSum += inputs[i].value * inputs[i].weight;
			}
		}

		output.value = (weightedSum + bias > 0 ? 1 : 0);
	}

	public override float Derivative()
	{
		return 0;
	}

	public override void UpdateWeights(float learnRate, float momentum)
	{
	}
}

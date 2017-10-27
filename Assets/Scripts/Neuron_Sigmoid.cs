//
// Written by Vander Roberto Nunes Dias, a.k.a. "imerso" / imersiva.com
//
// Sigmoid Neuron class
//

using System.Collections;
using System.Collections.Generic;

public class Neuron_Sigmoid : Neuron
{
	// Sigmoid constructor
	public Neuron_Sigmoid(float bias) : base(bias)
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

		weightedSum += bias;
		output.value = (float)(1.0 / (1.0 + System.Math.Exp(-weightedSum)));
	}

	public override float Derivative()
	{
		return (1 - output.value) * output.value;
	}

	public override void UpdateWeights(float learnRate, float momentum)
	{
		float prevDelta = biasDelta;
		biasDelta = learnRate * gradient;
		bias += biasDelta + momentum * prevDelta;

		for (int i = 0; i < inputs.Count; i++)
		{
			prevDelta = inputs[i].weightDelta;
			inputs[i].weightDelta = learnRate * gradient * inputs[i].value;
			inputs[i].weight += inputs[i].weightDelta + momentum * prevDelta;
		}
	}
}


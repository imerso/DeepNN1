//
// Written by Vander Roberto Nunes Dias, a.k.a. "imerso" / imersiva.com
//
// Base Neuron class
//
// (yes, I know get/set, just not obsessed)
//

using System.Collections;
using System.Collections.Generic;

// A NeuronDentrite is an electric terminal which holds a list of synapses,
// and can be used in both inputs and outputs
public class NeuronDentrite
{
	public List<NeuronDentrite> synapses = new List<NeuronDentrite>();
	public float weight;
	public float value;
	public Neuron neuron;		// to which neuron the dentrite pertains

	// for training
	public float weightDelta;

	// output dentrites don't need either weight and value specified.
	public NeuronDentrite(Neuron neuron, float weight = 0, float value = 0)
	{
		this.neuron = neuron;
		this.weight = weight;
		this.value = value;
	}
}

// The base Neuron class, which holds a list of input dentrites,
// but only one output dentrite
public abstract class Neuron
{
	// base neuron inputs and outputs, or synapses
	public List<NeuronDentrite> inputs = new List<NeuronDentrite>();
	public NeuronDentrite output;
	public float bias;
	public UnityEngine.Vector3 position;			// ### for unity3d rendering

	// for training
	public float error;
	protected float gradient;
	protected float biasDelta;


	// Neuron constructor
	public Neuron(float bias)
	{
		this.output = new NeuronDentrite(this);
		this.bias = bias;
	}

	// Update Neuron error Gradient
	public void UpdateGradient(float? desiredOutput = null)
	{
		if (desiredOutput == null)
		{
			// hidden layer
			float weightedGradient = 0;

			for (int i = 0; i < output.synapses.Count; i++)
			{
				weightedGradient += output.synapses[i].neuron.gradient * output.synapses[i].weight;
			}

			gradient = weightedGradient * Derivative();
		}
		else
		{
			// output layer
			error = desiredOutput.Value - output.value;
			gradient = error * Derivative();
		}
	}

	// Process inputs and output -- neuron dependent implementation
	public abstract void Update();
	public abstract float Derivative();
	public abstract void UpdateWeights(float learnRate, float momentum);
}

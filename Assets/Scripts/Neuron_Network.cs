//
// Written by Vander Roberto Nunes Dias, a.k.a. "imerso" / imersiva.com
//
// Neuron Network class
// A helper to initialize a full network with interconnected layers of neurons.
//

using System.Collections;
using System.Collections.Generic;

public class Neuron_Network
{
	public enum enNeuronType { perceptron, sigmoid }
	public enum enTopology { feedforward }
	public enum enMode { training, simulating }
	public float learnRate = 0.4f;
	public float momentum = 0.9f;

	enMode mode = enMode.training;
	enNeuronType neuronType;
	enTopology networkTopology;
	System.Random rnd = new System.Random();

	public List<List<Neuron>> layers = new List<List<Neuron>>();
	public List<float> trainingLayer = new List<float>();
	public float totalError;


	public Neuron_Network(enNeuronType neuronType, enTopology networkTopology, float learnRate, float momentum, int inputLayerNeuronCount, int hiddenLayersAmount, int hiddenLayersNeuronCount, int outputLayerNeuronCount)
	{
		this.neuronType = neuronType;
		this.networkTopology = networkTopology;
		this.learnRate = learnRate;
		this.momentum = momentum;

		// initialize input layer
		AddLayer(inputLayerNeuronCount);

		// initialize hidden layers
		for (int i = 0; i < hiddenLayersAmount; i++)
		{
			AddLayer(hiddenLayersNeuronCount);
		}

		// initialize output layer
		AddLayer(outputLayerNeuronCount);
		for (int i = 0; i < outputLayerNeuronCount; i++) trainingLayer.Add(0);

		// initialize connection topology
		switch (networkTopology)
		{
			case enTopology.feedforward:
				{
					// in feedforward, all previous layer outputs connect to all next layer inputs

					// loop from second layer until the last, connecting previous layer outputs to all current layer inputs
					for (int i = 1; i < layers.Count; i++)
					{
						List<Neuron> previousList = layers[i - 1];
						List<Neuron> currentList = layers[i];
						for (int pl = 0; pl < previousList.Count; pl++)
						{
							for (int cl = 0; cl < currentList.Count; cl++)
							{
								NeuronDentrite input = new NeuronDentrite(currentList[cl], -0.5f + (float)rnd.NextDouble());
								input.synapses.Add(previousList[pl].output);
								currentList[cl].inputs.Add(input);
								previousList[pl].output.synapses.Add(input);		// for backward propagation
							}
						}
					}

					break;
				}
		}
	}

	// set current operation mode
	public void SetMode(enMode newMode)
	{
		mode = newMode;
	}

	// update the network
	public void Update()
	{
		// loop through all neuron layers -- skip input layer
		for (int i = 1; i < layers.Count; i++)
		{
			// update each neuron in current layer
			List<Neuron> currentList = layers[i];
			for (int j = 0; j < currentList.Count; j++)
			{
				currentList[j].Update();
			}
		}

		switch (mode)
		{
			case enMode.training:
				{
					// calculate error
					totalError = 0;

					for (int i = layers.Count - 1; i > 0; i--)
					{
						List<Neuron> currentList = layers[i];

						for (int j = 0; j < currentList.Count; j++)
						{
							if (i == layers.Count-1) currentList[j].UpdateGradient(trainingLayer[j]);
							else currentList[j].UpdateGradient();

							currentList[j].UpdateWeights(learnRate, momentum);
							totalError += currentList[j].error;
						}

						//totalError /= currentList.Count;
					}
				}
				break;

			case enMode.simulating:
				{
				}
				break;
		}
	}

	void AddLayer(int neuronCount)
	{
		List<Neuron> list = new List<Neuron>();
		for (int i = 0; i < neuronCount; i++)
		{
			list.Add(NewNeuron(neuronType));
		}
		layers.Add(list);
	}

	Neuron NewNeuron(enNeuronType neuronType)
	{
		switch (neuronType)
		{
			case enNeuronType.perceptron:
				return new Neuron_Perceptron(-0.5f + (float)rnd.NextDouble());

			default:
			case enNeuronType.sigmoid:
				return new Neuron_Sigmoid(-0.5f + (float)rnd.NextDouble());
		}
	}
}

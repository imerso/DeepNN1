//
// Written by Vander Roberto Nunes Dias, a.k.a. "imerso" / imersiva.com
//
// Neuron Network Test -- MNIST digits training/recognition
// loops through 60000 mnist images training the network,
// then loops through 10000 different mnist images trying to recognize them.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Test_Network : MonoBehaviour
{
	public float learnRate = 0.4f;
	public float momentum = 0.9f;
	public Text debugText;
	public Text debugText2;
	public RawImage debugRawImage;

	Neuron_Network net;
	MnistReader mnist;
	bool isTraining = true;

	Texture2D digitTex;
	Color32[] digitPixels;
	float[] lastResult = new float[10];

	int epoch = 0;
	int rights = 0;
	int wrongs = 0;
	int totalCycles = 0;

	// initialize the network
	void Start()
	{
		// open mnist train images
		mnist = new MnistReader();
		mnist.OpenSet(Application.dataPath + "/Resources/train-labels.idx1-ubyte", Application.dataPath + "/Resources/train-images.idx3-ubyte");

		digitTex = new Texture2D(mnist.ImageWidth, mnist.ImageHeight);
		digitTex.Apply();
		digitPixels = new Color32[mnist.ImageLength];
		for (int i = 0; i < digitPixels.Length; i++) digitPixels[i] = new Color32(0,0,0,255);

		net = new Neuron_Network(Neuron_Network.enNeuronType.sigmoid, Neuron_Network.enTopology.feedforward, learnRate, momentum, 784, 1, 16, 10);
		net.SetMode(Neuron_Network.enMode.training);
		//CreateNetworkMesh();
	}

	// update
	void Update()
	{
		UnityEngine.Profiling.Profiler.BeginSample("Neuron Network Update");

		if (isTraining)
		{
			if (mnist.Current > 256)
			{
				epoch++;
				if (epoch < 1000)
				{
					mnist.Restart();
				}
			}

			// read next MNIST image
			byte label = mnist.ReadNextLabel();
			byte[] image = mnist.ReadNextImage();

			if (label == 255 || image == null || epoch >= 1000)
			{
				// end of images - stop training
				isTraining = false;
				net.SetMode(Neuron_Network.enMode.simulating);
				Debug.Log("END OF TRAINING.");
			}
			else
			{
				// copy image pixels to input neurons
				int len = mnist.ImageLength;
				List<Neuron> inputLayer = net.layers[0];
				for (int i = 0; i < len; i++)
				{
					// convert to range 0..1 (0=white, 1=black)
					inputLayer[i].output.value = (float)image[i] / 255;
					digitPixels[i].r = digitPixels[i].g = digitPixels[i].b = image[i]; 
				}
				digitTex.SetPixels32(digitPixels);
				digitTex.Apply();
				debugRawImage.texture = digitTex;

				// set only the desired training output to 1
				for (int i = 0; i < net.trainingLayer.Count; i++)
				{
					net.trainingLayer[i] = 0;
				}
				net.trainingLayer[label] = 1;

				// update the network
				net.Update();

				lastResult[label] = net.layers[net.layers.Count-1][label].output.value;

				debugText.text = epoch + " training " + mnist.Current + ": " + label + " error: " + net.totalError;
				string s = "";
				List<Neuron> outputLayer = net.layers[net.layers.Count - 1];
				for (int i = 0; i < 10; i++)
				{
					if (Mathf.Abs(net.trainingLayer[i] - outputLayer[i].output.value) < 0.5f)
					{
						s += "<color=#00FF00>";
					}
					else
					{
						s += "<color=#FF0000>";
					}

					s += i + ": desired " + (net.trainingLayer[i]) + "\t\t\t\tlearnt " + lastResult[i] + "\t\t\t\tframe " + (outputLayer[i].output.value) + "</color>\n";
				}
				debugText2.text = s;

				// check accuracy by creating a sorted outputLayer
				string color;
				List<Neuron> sortedOutputLayer = outputLayer.OrderByDescending(o => o.output.value).ToList();
				if (sortedOutputLayer[0].Equals(outputLayer[label]))
				{
					rights++;
					color = "#00FF00";
				}
				else
				{
					wrongs++;
					color = "#FF0000";
				}
				totalCycles++;
				debugText2.text += "\n" + "Cycles: " + totalCycles;
				debugText2.text += "\n<color=" + color + ">Rights: " + rights + "</color>\n" + "Wrongs: " + wrongs;
				float accuracy = (float)rights / totalCycles;
				debugText2.text += "\n" + "Accuracy: " + accuracy;
			}
		}
		else
		{
			// update the network
			net.Update();
		}

		UnityEngine.Profiling.Profiler.EndSample();
	}

	// create a visual representation of the network
	void CreateNetworkMesh()
	{
		float stepx = 6;
		float stepy = 2;

		float px = -(float)net.layers.Count / 2 * stepx;

		// loop through all neuron layers
		for (int i = 0; i < net.layers.Count; i++)
		{
			float py = (float)net.layers[i].Count / 2 * stepy;

			// create each neuron gameobject of current layer
			List<Neuron> currentList = net.layers[i];
			for (int j=0; j<currentList.Count; j++)
			{
				Neuron currentNeuron = currentList[j];

				// create neuron as a unit sphere
				GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				obj.isStatic = true;
				obj.name = "L" + i + "." + j;
				currentNeuron.position = new Vector3(px, py, 0);
				obj.transform.position = currentNeuron.position;

				if (i > 0)
				{
					// add line renderers connecting inputs to the previous layer outputs
					for (int k = 0; k < currentNeuron.inputs.Count; k++)
					{
						NeuronDentrite input = currentNeuron.inputs[k];
						for (int l = 0; l < input.synapses.Count; l++)
						{
							GameObject subObj = new GameObject("synapse");
							subObj.isStatic = true;
							subObj.transform.parent = obj.transform;
							subObj.transform.position = Vector3.zero;
							LineRenderer line = subObj.AddComponent<LineRenderer>();
							line.startWidth = 0.05f;
							line.endWidth = 0.05f;
							Vector3[] p = { obj.transform.position, input.synapses[l].neuron.position };
							line.SetPositions(p);
						}
					}
				}

				py -= stepy;
			}

			px += stepx;
		}
	}
}

﻿namespace DeepBelief

module MnistClassification =

    open Alea.CUDA
    open Alea.CUDA.Utilities
    open NeuralNet
    open Utils
    open MnistDataLoad
    open DeepBeliefNet
    open CudaTemplates

    let mnistTrainingImages = loadMnistImage MnistTrainingImageData
    let mnistTrainingLabels = loadMnistLabel MnistTrainingLabelData

    let mnistTestImages = loadMnistImage MnistTestImageData
    let mnistTestLabels = loadMnistLabel MnistTestLabelData

    let dbnSizes = [500; 250; 100; 50]
    let alpha = 0.5f
    let momentum = 0.9f

    let mnistDbn = dbn dbnSizes alpha momentum mnistTrainingImages
    //let trainedDbn = dbnTrain rand 100 10 mnistDbn mnistTrainingImages
    let trainedDbn = dbnTrain rand 100 2 mnistDbn mnistTrainingImages

    let rbmProps = 
        mnistDbn 
        |> List.map (fun rbm -> (prependColumn rbm.HiddenBiases rbm.Weights, sigmoid))
        |> List.unzip |> fun (weights, activations) -> { Weights = weights; Activations = activations }

    let props = { Weights = List.concat [|rbmProps.Weights; [initGaussianWeights 50 10]|]; Activations = sigmoid :: rbmProps.Activations }

    let trainingSet = Array.zip (toArray mnistTrainingImages) mnistTrainingLabels
    let testSet = Array.zip (toArray mnistTestImages) mnistTestLabels
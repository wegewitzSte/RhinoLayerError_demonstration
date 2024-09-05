namespace stuffFsharp

open System
open Rhino




module RhinoDocManagement =




    /// <summary>
    /// Creates a collection of layers and their children. Use this, to savely erase Layers, which children are empty.
    /// This function is the result of the RhinoCommon Api problem
    /// that the layer.GetChildren() function does not work!!. Instead i go around with the ParentID
    /// https://discourse.mcneel.com/t/extracting-the-layer-table-from-an-external-rhino-file/44352/10
    /// WARNING: This function sometimes does not contain all layers in a document. 
    ///          Check keys of they are contained in the collection before you get the value
    /// </summary>
    /// <param name="doc"></param>
    let findAllLayerChildren (doc: RhinoDoc) = 
        
        let collection =  System.Collections.Generic.Dictionary<Guid, ResizeArray<Guid>>()

        let addToCollection key value =
            
            if collection.ContainsKey(key) then
                collection[key].Add(value)
            else
                let newArr =  ResizeArray<Guid>()
                newArr.Add(value)
                collection.Add(key, newArr) 


        let addEmpty key  =
            if collection.ContainsKey(key) then
                ()
            else
                let newArr =  ResizeArray<Guid>()
                collection.Add(key, newArr) 


        for layer in doc.Layers do 

            let parentGuid = layer.ParentLayerId

            if parentGuid = Guid.Empty then
                addEmpty layer.Id // This is layer has no children
            else 
                addToCollection parentGuid layer.Id  // store parent child relationship

        collection 




    /// <summary>
    /// Remove every empty Layer from RhinoDoc
    /// </summary>
    /// <param name="doc"></param>
    let purgeEmptyLayers(doc: RhinoDoc, keepStandard: bool) = 
        

        let layerHierarchy = findAllLayerChildren (doc)

        let getSafely layerid = 
            
            if layerHierarchy.ContainsKey layerid then
                Some layerHierarchy.[layerid ]
            else    
                None

        // Hacky function to find the number of childobjects. Fuck this, but it kinda works.
        let rec getObjectNumberFromLayer (layer : Rhino.DocObjects.Layer) = 
            if layer = null || layer.IsDeleted = true then
                0
            else
                let obs = doc.Objects.FindByLayer(layer)
                let currentObs = if obs = null then 0 else obs.Length
                let childrenLayerGuids = getSafely layer.Id

                match childrenLayerGuids with
                | None -> currentObs
                | Some foundchildLayers ->
                    let childrenLayer = foundchildLayers |> Seq.map(fun g -> doc.Layers.FindId g) |> Seq.toList
           
                    let childrenObs =  childrenLayer |>  List.map(fun e -> getObjectNumberFromLayer  e) |> List.sum
                    currentObs + childrenObs 

        for layer in doc.Layers do 

            if layer.IsDeleted = false then

                let obs = doc.Objects.FindByLayer(layer)
        
                let childerLayerObjs =   getObjectNumberFromLayer layer
        
        
                if (obs = null || obs.Length = 0) && (childerLayerObjs <= 0) then
                        
                    if keepStandard = true && layer.Index = 0 then
                        ()
                    else
        
                        let result = doc.Layers.Purge(layer.Id, true)
                        match result with 
                        | true -> () 
                        | false -> RhinoApp.WriteLine $"Couldn't erase layer {layer.Name} with layer.id {layer.Id.ToString()}" 





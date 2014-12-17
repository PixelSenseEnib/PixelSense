Arborescence du Projet :

- Plane.cs 			// Classe Plane, définit un avion (Nom, Position, Rotation)
- Behaviour.cs 			// Classe Behaviour, permet l'envoi des informations au serveur
- PlaneTagVisualization.xaml 	// Définit l'image à apparaître sur l'écran de la table à la détection d'un tag
- SurfaceWindowAircraftCarrier.xaml // Définit l'apparence de la SurfaceWindow, créée le TagVisualizer
- SurfaceWindowAircraftCarrier.xaml.cs // Coeur de l'application
|--- InitializeTags 			// Initialise les tags de 0 à nbTags, variable modifiable
|--- GetServerIp			// Récupère l'IP du serveur	
|--- OnVisualizationAdded		// Actions à effectuer lors de l'apparition d'un tag
|--- OnVisualizationMoved		// Actions à effectuer lors du déplacement d'un tag
|--- OnVisualizationRemoved		// Actions à effectuer lors de la suppression d'un tag



Pour changer l'IP du Server  : il faut modifier GetServerIP, la partie commentée est loa partie dynamique, sinon changer la valeur retourné en dur
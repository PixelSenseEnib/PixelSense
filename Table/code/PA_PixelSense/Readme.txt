Arborescence du Projet :

- Plane.cs 			// Classe Plane, d�finit un avion (Nom, Position, Rotation)
- Behaviour.cs 			// Classe Behaviour, permet l'envoi des informations au serveur
- PlaneTagVisualization.xaml 	// D�finit l'image � appara�tre sur l'�cran de la table � la d�tection d'un tag
- SurfaceWindowAircraftCarrier.xaml // D�finit l'apparence de la SurfaceWindow, cr��e le TagVisualizer
- SurfaceWindowAircraftCarrier.xaml.cs // Coeur de l'application
|--- InitializeTags 			// Initialise les tags de 0 � nbTags, variable modifiable
|--- GetServerIp			// R�cup�re l'IP du serveur	
|--- OnVisualizationAdded		// Actions � effectuer lors de l'apparition d'un tag
|--- OnVisualizationMoved		// Actions � effectuer lors du d�placement d'un tag
|--- OnVisualizationRemoved		// Actions � effectuer lors de la suppression d'un tag



Pour changer l'IP du Server  : il faut modifier GetServerIP, la partie comment�e est loa partie dynamique, sinon changer la valeur retourn� en dur
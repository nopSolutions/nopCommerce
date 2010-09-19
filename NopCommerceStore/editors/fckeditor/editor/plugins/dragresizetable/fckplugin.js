var FCKDragTableHandler =
{
	"_DragState" : 0,
	"_LeftCell" : null,
	"_RightCell" : null,
	"_MouseMoveMode" : 0,	// 0 - find candidate cells for resizing, 1 - drag to resize
	"_ResizeBar" : null,
	"_OriginalX" : null,
	"_MinimumX" : null,
	"_MaximumX" : null,
	"_LastX" : null,
	"_TableMap" : null,
	"_doc" : document,
	"_IsInsideNode" : function( w, domNode, pos )
	{
		var myCoords = FCKTools.GetWindowPosition( w, domNode ) ;
		var xMin = myCoords.x ;
		var yMin = myCoords.y ;
		var xMax = parseInt( xMin, 10 ) + parseInt( domNode.offsetWidth, 10 ) ;
		var yMax = parseInt( yMin, 10 ) + parseInt( domNode.offsetHeight, 10 ) ;
		if ( pos.x >= xMin && pos.x <= xMax && pos.y >= yMin && pos.y <= yMax )
			return true;
		return false;
	},
	"_GetBorderCells" : function( w, tableNode, tableMap, mouse )
	{
		// Enumerate all the cells in the table.
		var cells = [] ;
		for ( var i = 0 ; i < tableNode.rows.length ; i++ )
		{
			var r = tableNode.rows[i] ;
			for ( var j = 0 ; j < r.cells.length ; j++ )
				cells.push( r.cells[j] ) ;
		}

		if ( cells.length < 1 )
			return null ;

		// Get the cells whose right or left border is nearest to the mouse cursor's x coordinate.
		var minRxDist = null ;
		var lxDist = null ;
		var minYDist = null ;
		var rbCell = null ;
		var lbCell = null ;
		for ( var i = 0 ; i < cells.length ; i++ )
		{
			var pos = FCKTools.GetWindowPosition( w, cells[i] ) ;
			var rightX = pos.x + parseInt( cells[i].clientWidth, 10 ) ;
			var rxDist = mouse.x - rightX ;
			var yDist = mouse.y - ( pos.y + ( cells[i].clientHeight / 2 ) ) ;
			if ( minRxDist == null ||
					( Math.abs( rxDist ) <= Math.abs( minRxDist ) &&
					  ( minYDist == null || Math.abs( yDist ) <= Math.abs( minYDist ) ) ) )
			{
				minRxDist = rxDist ;
				minYDist = yDist ;
				rbCell = cells[i] ;
			}
		}
		/*
		var rowNode = FCKTools.GetElementAscensor( rbCell, "tr" ) ;
		var cellIndex = rbCell.cellIndex + 1 ;
		if ( cellIndex >= rowNode.cells.length )
			return null ;
		lbCell = rowNode.cells.item( cellIndex ) ;
		*/
		var rowIdx = rbCell.parentNode.rowIndex ;
		var colIdx = FCKTableHandler._GetCellIndexSpan( tableMap, rowIdx, rbCell ) ;
		var colSpan = isNaN( rbCell.colSpan ) ? 1 : rbCell.colSpan ;
		lbCell = tableMap[rowIdx][colIdx + colSpan] ;

		if ( ! lbCell )
			return null ;

		// Abort if too far from the border.
		lxDist = mouse.x - FCKTools.GetWindowPosition( w, lbCell ).x ;
		if ( lxDist < 0 && minRxDist < 0 && minRxDist < -2 )
			return null ;
		if ( lxDist > 0 && minRxDist > 0 && lxDist > 3 )
			return null ;

		return { "leftCell" : rbCell, "rightCell" : lbCell } ;
	},
	"_GetResizeBarPosition" : function()
	{
		var row = FCKTools.GetElementAscensor( this._RightCell, "tr" ) ;
		return FCKTableHandler._GetCellIndexSpan( this._TableMap, row.rowIndex, this._RightCell ) ;
	},
	"_ResizeBarMouseDownListener" : function( evt )
	{
		if ( FCKDragTableHandler._LeftCell )
			FCKDragTableHandler._MouseMoveMode = 1 ;
		if ( FCKBrowserInfo.IsIE )
			FCKDragTableHandler._ResizeBar.filters.item("DXImageTransform.Microsoft.Alpha").opacity = 50 ;
		else
			FCKDragTableHandler._ResizeBar.style.opacity = 0.5 ;
		FCKDragTableHandler._OriginalX = evt.clientX ;

		// Calculate maximum and minimum x-coordinate delta.
		var borderIndex = FCKDragTableHandler._GetResizeBarPosition() ;
		var offset = FCKDragTableHandler._GetIframeOffset();
		var table = FCKTools.GetElementAscensor( FCKDragTableHandler._LeftCell, "table" );
		var minX = null ;
		var maxX = null ;
		for ( var r = 0 ; r < FCKDragTableHandler._TableMap.length ; r++ )
		{
			var leftCell = FCKDragTableHandler._TableMap[r][borderIndex - 1] ;
			var rightCell = FCKDragTableHandler._TableMap[r][borderIndex] ;
			var leftPosition = FCKTools.GetWindowPosition( FCK.EditorWindow, leftCell ) ;
			var rightPosition = FCKTools.GetWindowPosition( FCK.EditorWindow, rightCell ) ;
			var leftPadding = FCKDragTableHandler._GetCellPadding( table, leftCell ) ;
			var rightPadding = FCKDragTableHandler._GetCellPadding( table, rightCell ) ;
			if ( minX == null || leftPosition.x + leftPadding > minX )
				minX = leftPosition.x + leftPadding ;
			if ( maxX == null || rightPosition.x + rightCell.clientWidth - rightPadding < maxX )
				maxX = rightPosition.x + rightCell.clientWidth - rightPadding ;
		}

		FCKDragTableHandler._MinimumX = minX + offset.x ;
		FCKDragTableHandler._MaximumX = maxX + offset.x ;
		FCKDragTableHandler._LastX = null ;

		if (evt.preventDefault)
			evt.preventDefault();
		else
			evt.returnValue = false;
	},
	"_ResizeBarMouseUpListener" : function( evt )
	{
		FCKDragTableHandler._MouseMoveMode = 0 ;
		FCKDragTableHandler._HideResizeBar() ;

		if ( FCKDragTableHandler._LastX == null )
			return ;

		// Calculate the delta value.
		var deltaX = FCKDragTableHandler._LastX - FCKDragTableHandler._OriginalX ;

		// Then, build an array of current column width values.
		// This algorithm can be very slow if the cells have insane colSpan values. (e.g. colSpan=1000).
		var table = FCKTools.GetElementAscensor( FCKDragTableHandler._LeftCell, "table" ) ;
		var colArray = [] ;
		var tableMap = FCKDragTableHandler._TableMap ;
		for ( var i = 0 ; i < tableMap.length ; i++ )
		{
			for ( var j = 0 ; j < tableMap[i].length ; j++ )
			{
				var cell = tableMap[i][j] ;
				var width = FCKDragTableHandler._GetCellWidth( table, cell ) ;
				var colSpan = isNaN( cell.colSpan) ? 1 : cell.colSpan ;
				if ( colArray.length <= j )
					colArray.push( { width : width / colSpan, colSpan : colSpan } ) ;
				else
				{
					var guessItem = colArray[j] ;
					if ( guessItem.colSpan > colSpan )
					{
						guessItem.width = width / colSpan ;
						guessItem.colSpan = colSpan ;
					}
				}
			}
		}

		// Find out the equivalent column index of the two cells selected for resizing.
		colIndex = FCKDragTableHandler._GetResizeBarPosition() ;

		// Note that colIndex must be at least 1 here, so it's safe to subtract 1 from it.
		colIndex-- ;

		// Modify the widths in the colArray according to the mouse coordinate delta value.
		colArray[colIndex].width += deltaX ;
		colArray[colIndex + 1].width -= deltaX ;

		// Clear all cell widths, delete all <col> elements from the table.
		for ( var r = 0 ; r < table.rows.length ; r++ )
		{
			var row = table.rows.item( r ) ;
			for ( var c = 0 ; c < row.cells.length ; c++ )
			{
				var cell = row.cells.item( c ) ;
				cell.width = "" ;
				cell.style.width = "" ;
			}
		}
		var colElements = table.getElementsByTagName( "col" ) ;
		for ( var i = colElements.length - 1 ; i >= 0 ; i-- )
			colElements[i].parentNode.removeChild( colElements[i] ) ;

		// Set new cell widths.
		var processedCells = [] ;
		for ( var i = 0 ; i < tableMap.length ; i++ )
		{
			for ( var j = 0 ; j < tableMap[i].length ; j++ )
			{
				var cell = tableMap[i][j] ;
				if ( cell._Processed )
					continue ;
				if ( tableMap[i][j-1] != cell )
					cell.width = colArray[j].width ;
				else
					cell.width = parseInt( cell.width, 10 ) + parseInt( colArray[j].width, 10 ) ;
				if ( tableMap[i][j+1] != cell )
				{
					processedCells.push( cell ) ;
					cell._Processed = true ;
				}
			}
		}
		for ( var i = 0 ; i < processedCells.length ; i++ )
		{
			if ( FCKBrowserInfo.IsIE )
				processedCells[i].removeAttribute( '_Processed' ) ;
			else
				delete processedCells[i]._Processed ;
		}

		FCKDragTableHandler._LastX = null ;
	},
	"_ResizeBarMouseMoveListener" : function( evt )
	{
		if ( FCKDragTableHandler._MouseMoveMode == 0 )
			return FCKDragTableHandler._MouseFindHandler( FCK, evt ) ;
		else
			return FCKDragTableHandler._MouseDragHandler( FCK, evt ) ;
	},
	// Calculate the padding of a table cell.
	// It returns the value of paddingLeft + paddingRight of a table cell.
	// This function is used, in part, to calculate the width parameter that should be used for setting cell widths.
	// The equation in question is clientWidth = paddingLeft + paddingRight + width.
	// So that width = clientWidth - paddingLeft - paddingRight.
	// The return value of this function must be pixel accurate acorss all supported browsers, so be careful if you need to modify it.
	"_GetCellPadding" : function( table, cell )
	{
		var attrGuess = parseInt( table.cellPadding, 10 ) * 2 ;
		var cssGuess = null ;
		if ( typeof( window.getComputedStyle ) == "function" )
		{
			var styleObj = window.getComputedStyle( cell, null ) ;
			cssGuess = parseInt( styleObj.getPropertyValue( "padding-left" ), 10 ) +
				parseInt( styleObj.getPropertyValue( "padding-right" ), 10 ) ;
		}
		else
			cssGuess = parseInt( cell.currentStyle.paddingLeft, 10 ) + parseInt (cell.currentStyle.paddingRight, 10 ) ;

		var cssRuntime = cell.style.padding ;
		if ( isFinite( cssRuntime ) )
			cssGuess = parseInt( cssRuntime, 10 ) * 2 ;
		else
		{
			cssRuntime = cell.style.paddingLeft ;
			if ( isFinite( cssRuntime ) )
				cssGuess = parseInt( cssRuntime, 10 ) ;
			cssRuntime = cell.style.paddingRight ;
			if ( isFinite( cssRuntime ) )
				cssGuess += parseInt( cssRuntime, 10 ) ;
		}

		attrGuess = parseInt( attrGuess, 10 ) ;
		cssGuess = parseInt( cssGuess, 10 ) ;
		if ( isNaN( attrGuess ) )
			attrGuess = 0 ;
		if ( isNaN( cssGuess ) )
			cssGuess = 0 ;
		return Math.max( attrGuess, cssGuess ) ;
	},
	// Calculate the real width of the table cell.
	// The real width of the table cell is the pixel width that you can set to the width attribute of the table cell and after
	// that, the table cell should be of exactly the same width as before.
	// The real width of a table cell can be calculated as:
	// width = clientWidth - paddingLeft - paddingRight.
	"_GetCellWidth" : function( table, cell )
	{
		var clientWidth = cell.clientWidth ;
		if ( isNaN( clientWidth ) )
			clientWidth = 0 ;
		return clientWidth - this._GetCellPadding( table, cell ) ;
	},
	"MouseMoveListener" : function( FCK, evt )
	{
		if ( FCKDragTableHandler._MouseMoveMode == 0 )
			return FCKDragTableHandler._MouseFindHandler( FCK, evt ) ;
		else
			return FCKDragTableHandler._MouseDragHandler( FCK, evt ) ;
	},
	"_MouseFindHandler" : function( FCK, evt )
	{
		if ( FCK.MouseDownFlag )
			return ;
		var node = evt.srcElement || evt.target ;
		try
		{
			if ( ! node || node.nodeType != 1 )
			{
				this._HideResizeBar() ;
				return ;
			}
		}
		catch ( e )
		{
			this._HideResizeBar() ;
			return ;
		}

		// Since this function might be called from the editing area iframe or the outer fckeditor iframe,
		// the mouse point coordinates from evt.clientX/Y can have different reference points.
		// We need to resolve the mouse pointer position relative to the editing area iframe.
		var mouseX = evt.clientX ;
		var mouseY = evt.clientY ;
		if ( FCKTools.GetElementDocument( node ) == document )
		{
			var offset = this._GetIframeOffset() ;
			mouseX -= offset.x ;
			mouseY -= offset.y ;
		}


		if ( this._ResizeBar && this._LeftCell )
		{
			var leftPos = FCKTools.GetWindowPosition( FCK.EditorWindow, this._LeftCell ) ;
			var rightPos = FCKTools.GetWindowPosition( FCK.EditorWindow, this._RightCell ) ;
			var rxDist = mouseX - ( leftPos.x + this._LeftCell.clientWidth ) ;
			var lxDist = mouseX - rightPos.x ;
			var inRangeFlag = false ;
			if ( lxDist >= 0 && rxDist <= 0 )
				inRangeFlag = true ;
			else if ( rxDist > 0 && lxDist <= 3 )
				inRangeFlag = true ;
			else if ( lxDist < 0 && rxDist >= -2 )
				inRangeFlag = true ;
			if ( inRangeFlag )
			{
				this._ShowResizeBar( FCK.EditorWindow,
					FCKTools.GetElementAscensor( this._LeftCell, "table" ),
					{ "x" : mouseX, "y" : mouseY } ) ;
				return ;
			}
		}

		var tagName = node.tagName.toLowerCase() ;
		if ( tagName != "table" && tagName != "td" && tagName != "th" )
		{
			if ( this._LeftCell )
				this._LeftCell = this._RightCell = this._TableMap = null ;
			this._HideResizeBar() ;
			return ;
		}
		node = FCKTools.GetElementAscensor( node, "table" ) ;
		var tableMap = FCKTableHandler._CreateTableMap( node ) ;
		var cellTuple = this._GetBorderCells( FCK.EditorWindow, node, tableMap, { "x" : mouseX, "y" : mouseY } ) ;

		if ( cellTuple == null )
		{
			if ( this._LeftCell )
				this._LeftCell = this._RightCell = this._TableMap = null ;
			this._HideResizeBar() ;
		}
		else
		{
			this._LeftCell = cellTuple["leftCell"] ;
			this._RightCell = cellTuple["rightCell"] ;
			this._TableMap = tableMap ;
			this._ShowResizeBar( FCK.EditorWindow,
					FCKTools.GetElementAscensor( this._LeftCell, "table" ),
					{ "x" : mouseX, "y" : mouseY } ) ;
		}
	},
	"_MouseDragHandler" : function( FCK, evt )
	{
		var mouse = { "x" : evt.clientX, "y" : evt.clientY } ;

		// Convert mouse coordinates in reference to the outer iframe.
		var node = evt.srcElement || evt.target ;
		if ( FCKTools.GetElementDocument( node ) == FCK.EditorDocument )
		{
			var offset = this._GetIframeOffset() ;
			mouse.x += offset.x ;
			mouse.y += offset.y ;
		}

		// Calculate the mouse position delta and see if we've gone out of range.
		if ( mouse.x >= this._MaximumX - 5 )
			mouse.x = this._MaximumX - 5 ;
		if ( mouse.x <= this._MinimumX + 5 )
			mouse.x = this._MinimumX + 5 ;

		var docX = mouse.x + FCKTools.GetScrollPosition( window ).X ;
		this._ResizeBar.style.left = ( docX - this._ResizeBar.offsetWidth / 2 ) + "px" ;
		this._LastX = mouse.x ;
	},
	"_ShowResizeBar" : function( w, table, mouse )
	{
		if ( this._ResizeBar == null )
		{
			this._ResizeBar = this._doc.createElement( "div" ) ;
			var paddingBar = this._ResizeBar ;
			var paddingStyles = { 'position' : 'absolute', 'cursor' : 'e-resize' } ;
			if ( FCKBrowserInfo.IsIE )
				paddingStyles.filter = "progid:DXImageTransform.Microsoft.Alpha(opacity=10,enabled=true)" ;
			else
				paddingStyles.opacity = 0.10 ;
			FCKDomTools.SetElementStyles( paddingBar, paddingStyles ) ;
			this._avoidStyles( paddingBar );
			paddingBar.setAttribute('_fcktemp', true);
			this._doc.body.appendChild( paddingBar ) ;
			FCKTools.AddEventListener( paddingBar, "mousemove", this._ResizeBarMouseMoveListener ) ;
			FCKTools.AddEventListener( paddingBar, "mousedown", this._ResizeBarMouseDownListener ) ;
			FCKTools.AddEventListener( document, "mouseup", this._ResizeBarMouseUpListener ) ;
			FCKTools.AddEventListener( FCK.EditorDocument, "mouseup", this._ResizeBarMouseUpListener ) ;

			// IE doesn't let the tranparent part of the padding block to receive mouse events unless there's something inside.
			// So we need to create a spacer image to fill the block up.
			var filler = this._doc.createElement( "img" ) ;
			filler.setAttribute('_fcktemp', true);
			filler.border = 0 ;
			filler.src = FCKConfig.BasePath + "images/spacer.gif" ;
			filler.style.position = "absolute" ;
			paddingBar.appendChild( filler ) ;

			// Disable drag and drop, and selection for the filler image.
			var disabledListener = function( evt )
			{
				if ( evt.preventDefault )
					evt.preventDefault() ;
				else
					evt.returnValue = false ;
			}
			FCKTools.AddEventListener( filler, "dragstart", disabledListener ) ;
			FCKTools.AddEventListener( filler, "selectstart", disabledListener ) ;
		}

		var paddingBar = this._ResizeBar ;
		var offset = this._GetIframeOffset() ;
		var tablePos = this._GetTablePosition( w, table ) ;
		var barHeight = table.offsetHeight ;
		var barTop = offset.y + tablePos.y ;
		// Do not let the resize bar intrude into the toolbar area.
		if ( tablePos.y < 0 )
		{
			barHeight += tablePos.y ;
			barTop -= tablePos.y ;
		}
		var bw = parseInt( table.border, 10 ) ;
		if ( isNaN( bw ) )
			bw = 0 ;
		var cs = parseInt( table.cellSpacing, 10 ) ;
		if ( isNaN( cs ) )
			cs = 0 ;
		var barWidth = Math.max( bw+100, cs+100 ) ;
		var paddingStyles =
		{
			'top'		: barTop + 'px',
			'height'	: barHeight + 'px',
			'width'		: barWidth + 'px',
			'left'		: ( offset.x + mouse.x + FCKTools.GetScrollPosition( w ).X - barWidth / 2 ) + 'px'
		} ;
		if ( FCKBrowserInfo.IsIE )
			paddingBar.filters.item("DXImageTransform.Microsoft.Alpha").opacity = 10 ;
		else
			paddingStyles.opacity = 0.1 ;

		FCKDomTools.SetElementStyles( paddingBar, paddingStyles ) ;
		var filler = paddingBar.getElementsByTagName( "img" )[0] ;

		FCKDomTools.SetElementStyles( filler,
			{
				width	: paddingBar.offsetWidth + 'px',
				height	: barHeight + 'px'
			} ) ;

		barWidth = Math.max( bw, cs, 3 ) ;
		var visibleBar = null ;
		if ( paddingBar.getElementsByTagName( "div" ).length < 1 )
		{
			visibleBar = this._doc.createElement( "div" ) ;
			this._avoidStyles( visibleBar );
			visibleBar.setAttribute('_fcktemp', true);
			paddingBar.appendChild( visibleBar ) ;
		}
		else
			visibleBar = paddingBar.getElementsByTagName( "div" )[0] ;

		FCKDomTools.SetElementStyles( visibleBar,
			{
				position		: 'absolute',
				backgroundColor	: 'blue',
				width			: barWidth + 'px',
				height			: barHeight + 'px',
				left			: '50px',
				top				: '0px'
			} ) ;
	},
	"_HideResizeBar" : function()
	{
		if ( this._ResizeBar )
			// IE bug: display : none does not hide the resize bar for some reason.
			// so set the position to somewhere invisible.
			FCKDomTools.SetElementStyles( this._ResizeBar,
				{
					top		: '-100000px',
					left	: '-100000px'
				} ) ;
	},
	"_GetIframeOffset" : function ()
	{
		return FCKTools.GetDocumentPosition( window, FCK.EditingArea.IFrame ) ;
	},
	"_GetTablePosition" : function ( w, table )
	{
		return FCKTools.GetWindowPosition( w, table ) ;
	},
	"_avoidStyles" : function( element )
	{
		FCKDomTools.SetElementStyles( element,
			{
				padding		: '0',
				backgroundImage	: 'none',
				border		: '0'
			} ) ;
	},
	"Reset" : function()
	{
		FCKDragTableHandler._LeftCell = FCKDragTableHandler._RightCell = FCKDragTableHandler._TableMap = null ;
	}

};

FCK.Events.AttachEvent( "OnMouseMove", FCKDragTableHandler.MouseMoveListener ) ;
FCK.Events.AttachEvent( "OnAfterSetHTML", FCKDragTableHandler.Reset ) ;

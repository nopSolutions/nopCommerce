/*
* This file has been generated to support Visual Studio IntelliSense.
* You should not use this file at runtime inside the browser--it is only
* intended to be used only for design-time IntelliSense.  Please use the
* standard jQuery library for all runtime use.
*
* Comment version: 1.10.2
*/

/*!
* jQuery JavaScript Library v1.10.2
* http://jquery.com/
*
* Includes Sizzle.js
* http://sizzlejs.com/
*
* Copyright 2005, 2012 jQuery Foundation, Inc. and other contributors
* Released under the MIT license
* http://jquery.org/license
*
*/

(function ( window, undefined ) {
var jQuery = function( selector, context ) {
/// <summary>
///     1: Accepts a string containing a CSS selector which is then used to match a set of elements.
///     &#10;    1.1 - $(selector, context) 
///     &#10;    1.2 - $(element) 
///     &#10;    1.3 - $(object) 
///     &#10;    1.4 - $(elementArray) 
///     &#10;    1.5 - $(jQuery object) 
///     &#10;    1.6 - $()
///     &#10;2: Creates DOM elements on the fly from the provided string of raw HTML.
///     &#10;    2.1 - $(html, ownerDocument) 
///     &#10;    2.2 - $(html, props)
///     &#10;3: Binds a function to be executed when the DOM has finished loading.
///     &#10;    3.1 - $(callback)
/// </summary>
/// <param name="selector" type="String">
///     A string containing a selector expression
/// </param>
/// <param name="context" type="jQuery">
///     A DOM Element, Document, or jQuery to use as context
/// </param>
/// <returns type="jQuery" />

		// The jQuery object is actually just the init constructor 'enhanced'
		return new jQuery.fn.init( selector, context, rootjQuery );
	};
jQuery.Animation = function Animation( elem, properties, options ) {

	var result,
		stopped,
		index = 0,
		length = animationPrefilters.length,
		deferred = jQuery.Deferred().always( function() {
			// don't match elem in the :animated selector
			delete tick.elem;
		}),
		tick = function() {
			if ( stopped ) {
				return false;
			}
			var currentTime = fxNow || createFxNow(),
				remaining = Math.max( 0, animation.startTime + animation.duration - currentTime ),
				// archaic crash bug won't allow us to use 1 - ( 0.5 || 0 ) (#12497)
				temp = remaining / animation.duration || 0,
				percent = 1 - temp,
				index = 0,
				length = animation.tweens.length;

			for ( ; index < length ; index++ ) {
				animation.tweens[ index ].run( percent );
			}

			deferred.notifyWith( elem, [ animation, percent, remaining ]);

			if ( percent < 1 && length ) {
				return remaining;
			} else {
				deferred.resolveWith( elem, [ animation ] );
				return false;
			}
		},
		animation = deferred.promise({
			elem: elem,
			props: jQuery.extend( {}, properties ),
			opts: jQuery.extend( true, { specialEasing: {} }, options ),
			originalProperties: properties,
			originalOptions: options,
			startTime: fxNow || createFxNow(),
			duration: options.duration,
			tweens: [],
			createTween: function( prop, end ) {
				var tween = jQuery.Tween( elem, animation.opts, prop, end,
						animation.opts.specialEasing[ prop ] || animation.opts.easing );
				animation.tweens.push( tween );
				return tween;
			},
			stop: function( gotoEnd ) {
				var index = 0,
					// if we are going to the end, we want to run all the tweens
					// otherwise we skip this part
					length = gotoEnd ? animation.tweens.length : 0;
				if ( stopped ) {
					return this;
				}
				stopped = true;
				for ( ; index < length ; index++ ) {
					animation.tweens[ index ].run( 1 );
				}

				// resolve when we played the last frame
				// otherwise, reject
				if ( gotoEnd ) {
					deferred.resolveWith( elem, [ animation, gotoEnd ] );
				} else {
					deferred.rejectWith( elem, [ animation, gotoEnd ] );
				}
				return this;
			}
		}),
		props = animation.props;

	propFilter( props, animation.opts.specialEasing );

	for ( ; index < length ; index++ ) {
		result = animationPrefilters[ index ].call( animation, elem, props, animation.opts );
		if ( result ) {
			return result;
		}
	}

	jQuery.map( props, createTween, animation );

	if ( jQuery.isFunction( animation.opts.start ) ) {
		animation.opts.start.call( elem, animation );
	}

	jQuery.fx.timer(
		jQuery.extend( tick, {
			elem: elem,
			anim: animation,
			queue: animation.opts.queue
		})
	);

	// attach callbacks from options
	return animation.progress( animation.opts.progress )
		.done( animation.opts.done, animation.opts.complete )
		.fail( animation.opts.fail )
		.always( animation.opts.always );
};
jQuery.Callbacks = function( options ) {
/// <summary>
///     A multi-purpose callbacks list object that provides a powerful way to manage callback lists.
/// </summary>
/// <param name="options" type="String">
///     An optional list of space-separated flags that change how the callback list behaves.
/// </param>


	// Convert options from String-formatted to Object-formatted if needed
	// (we check in cache first)
	options = typeof options === "string" ?
		( optionsCache[ options ] || createOptions( options ) ) :
		jQuery.extend( {}, options );

	var // Flag to know if list is currently firing
		firing,
		// Last fire value (for non-forgettable lists)
		memory,
		// Flag to know if list was already fired
		fired,
		// End of the loop when firing
		firingLength,
		// Index of currently firing callback (modified by remove if needed)
		firingIndex,
		// First callback to fire (used internally by add and fireWith)
		firingStart,
		// Actual callback list
		list = [],
		// Stack of fire calls for repeatable lists
		stack = !options.once && [],
		// Fire callbacks
		fire = function( data ) {
			memory = options.memory && data;
			fired = true;
			firingIndex = firingStart || 0;
			firingStart = 0;
			firingLength = list.length;
			firing = true;
			for ( ; list && firingIndex < firingLength; firingIndex++ ) {
				if ( list[ firingIndex ].apply( data[ 0 ], data[ 1 ] ) === false && options.stopOnFalse ) {
					memory = false; // To prevent further calls using add
					break;
				}
			}
			firing = false;
			if ( list ) {
				if ( stack ) {
					if ( stack.length ) {
						fire( stack.shift() );
					}
				} else if ( memory ) {
					list = [];
				} else {
					self.disable();
				}
			}
		},
		// Actual Callbacks object
		self = {
			// Add a callback or a collection of callbacks to the list
			add: function() {
				if ( list ) {
					// First, we save the current length
					var start = list.length;
					(function add( args ) {
						jQuery.each( args, function( _, arg ) {
							var type = jQuery.type( arg );
							if ( type === "function" ) {
								if ( !options.unique || !self.has( arg ) ) {
									list.push( arg );
								}
							} else if ( arg && arg.length && type !== "string" ) {
								// Inspect recursively
								add( arg );
							}
						});
					})( arguments );
					// Do we need to add the callbacks to the
					// current firing batch?
					if ( firing ) {
						firingLength = list.length;
					// With memory, if we're not firing then
					// we should call right away
					} else if ( memory ) {
						firingStart = start;
						fire( memory );
					}
				}
				return this;
			},
			// Remove a callback from the list
			remove: function() {
				if ( list ) {
					jQuery.each( arguments, function( _, arg ) {
						var index;
						while( ( index = jQuery.inArray( arg, list, index ) ) > -1 ) {
							list.splice( index, 1 );
							// Handle firing indexes
							if ( firing ) {
								if ( index <= firingLength ) {
									firingLength--;
								}
								if ( index <= firingIndex ) {
									firingIndex--;
								}
							}
						}
					});
				}
				return this;
			},
			// Check if a given callback is in the list.
			// If no argument is given, return whether or not list has callbacks attached.
			has: function( fn ) {
				return fn ? jQuery.inArray( fn, list ) > -1 : !!( list && list.length );
			},
			// Remove all callbacks from the list
			empty: function() {
				list = [];
				firingLength = 0;
				return this;
			},
			// Have the list do nothing anymore
			disable: function() {
				list = stack = memory = undefined;
				return this;
			},
			// Is it disabled?
			disabled: function() {
				return !list;
			},
			// Lock the list in its current state
			lock: function() {
				stack = undefined;
				if ( !memory ) {
					self.disable();
				}
				return this;
			},
			// Is it locked?
			locked: function() {
				return !stack;
			},
			// Call all callbacks with the given context and arguments
			fireWith: function( context, args ) {
				args = args || [];
				args = [ context, args.slice ? args.slice() : args ];
				if ( list && ( !fired || stack ) ) {
					if ( firing ) {
						stack.push( args );
					} else {
						fire( args );
					}
				}
				return this;
			},
			// Call all the callbacks with the given arguments
			fire: function() {
				self.fireWith( this, arguments );
				return this;
			},
			// To know if the callbacks have already been called at least once
			fired: function() {
				return !!fired;
			}
		};

	return self;
};
jQuery.Deferred = function( func ) {

		var tuples = [
				// action, add listener, listener list, final state
				[ "resolve", "done", jQuery.Callbacks("once memory"), "resolved" ],
				[ "reject", "fail", jQuery.Callbacks("once memory"), "rejected" ],
				[ "notify", "progress", jQuery.Callbacks("memory") ]
			],
			state = "pending",
			promise = {
				state: function() {
					return state;
				},
				always: function() {
					deferred.done( arguments ).fail( arguments );
					return this;
				},
				then: function( /* fnDone, fnFail, fnProgress */ ) {
					var fns = arguments;
					return jQuery.Deferred(function( newDefer ) {
						jQuery.each( tuples, function( i, tuple ) {
							var action = tuple[ 0 ],
								fn = jQuery.isFunction( fns[ i ] ) && fns[ i ];
							// deferred[ done | fail | progress ] for forwarding actions to newDefer
							deferred[ tuple[1] ](function() {
								var returned = fn && fn.apply( this, arguments );
								if ( returned && jQuery.isFunction( returned.promise ) ) {
									returned.promise()
										.done( newDefer.resolve )
										.fail( newDefer.reject )
										.progress( newDefer.notify );
								} else {
									newDefer[ action + "With" ]( this === promise ? newDefer.promise() : this, fn ? [ returned ] : arguments );
								}
							});
						});
						fns = null;
					}).promise();
				},
				// Get a promise for this deferred
				// If obj is provided, the promise aspect is added to the object
				promise: function( obj ) {
					return obj != null ? jQuery.extend( obj, promise ) : promise;
				}
			},
			deferred = {};

		// Keep pipe for back-compat
		promise.pipe = promise.then;

		// Add list-specific methods
		jQuery.each( tuples, function( i, tuple ) {
			var list = tuple[ 2 ],
				stateString = tuple[ 3 ];

			// promise[ done | fail | progress ] = list.add
			promise[ tuple[1] ] = list.add;

			// Handle state
			if ( stateString ) {
				list.add(function() {
					// state = [ resolved | rejected ]
					state = stateString;

				// [ reject_list | resolve_list ].disable; progress_list.lock
				}, tuples[ i ^ 1 ][ 2 ].disable, tuples[ 2 ][ 2 ].lock );
			}

			// deferred[ resolve | reject | notify ]
			deferred[ tuple[0] ] = function() {
				deferred[ tuple[0] + "With" ]( this === deferred ? promise : this, arguments );
				return this;
			};
			deferred[ tuple[0] + "With" ] = list.fireWith;
		});

		// Make the deferred a promise
		promise.promise( deferred );

		// Call given func if any
		if ( func ) {
			func.call( deferred, deferred );
		}

		// All done!
		return deferred;
	};
jQuery.Event = function( src, props ) {

	// Allow instantiation without the 'new' keyword
	if ( !(this instanceof jQuery.Event) ) {
		return new jQuery.Event( src, props );
	}

	// Event object
	if ( src && src.type ) {
		this.originalEvent = src;
		this.type = src.type;

		// Events bubbling up the document may have been marked as prevented
		// by a handler lower down the tree; reflect the correct value.
		this.isDefaultPrevented = ( src.defaultPrevented || src.returnValue === false ||
			src.getPreventDefault && src.getPreventDefault() ) ? returnTrue : returnFalse;

	// Event type
	} else {
		this.type = src;
	}

	// Put explicitly provided properties onto the event object
	if ( props ) {
		jQuery.extend( this, props );
	}

	// Create a timestamp if incoming event doesn't have one
	this.timeStamp = src && src.timeStamp || jQuery.now();

	// Mark it as fixed
	this[ jQuery.expando ] = true;
};
jQuery.Tween = function Tween( elem, options, prop, end, easing ) {

	return new Tween.prototype.init( elem, options, prop, end, easing );
};
jQuery._data = function( elem, name, data ) {

		return internalData( elem, name, data, true );
	};
jQuery._evalUrl = function( url ) {

		return jQuery.ajax({
			url: url,
			type: "GET",
			dataType: "script",
			async: false,
			global: false,
			"throws": true
		});
	};
jQuery._queueHooks = function( elem, type ) {

		var key = type + "queueHooks";
		return jQuery._data( elem, key ) || jQuery._data( elem, key, {
			empty: jQuery.Callbacks("once memory").add(function() {
				jQuery._removeData( elem, type + "queue" );
				jQuery._removeData( elem, key );
			})
		});
	};
jQuery._removeData = function( elem, name ) {

		return internalRemoveData( elem, name, true );
	};
jQuery.acceptData = function( elem ) {

		// Do not set data on non-element because it will not be cleared (#8335).
		if ( elem.nodeType && elem.nodeType !== 1 && elem.nodeType !== 9 ) {
			return false;
		}

		var noData = elem.nodeName && jQuery.noData[ elem.nodeName.toLowerCase() ];

		// nodes accept data unless otherwise specified; rejection can be conditional
		return !noData || noData !== true && elem.getAttribute("classid") === noData;
	};
jQuery.access = function( elems, fn, key, value, chainable, emptyGet, raw ) {

		var i = 0,
			length = elems.length,
			bulk = key == null;

		// Sets many values
		if ( jQuery.type( key ) === "object" ) {
			chainable = true;
			for ( i in key ) {
				jQuery.access( elems, fn, i, key[i], true, emptyGet, raw );
			}

		// Sets one value
		} else if ( value !== undefined ) {
			chainable = true;

			if ( !jQuery.isFunction( value ) ) {
				raw = true;
			}

			if ( bulk ) {
				// Bulk operations run against the entire set
				if ( raw ) {
					fn.call( elems, value );
					fn = null;

				// ...except when executing function values
				} else {
					bulk = fn;
					fn = function( elem, key, value ) {
						return bulk.call( jQuery( elem ), value );
					};
				}
			}

			if ( fn ) {
				for ( ; i < length; i++ ) {
					fn( elems[i], key, raw ? value : value.call( elems[i], i, fn( elems[i], key ) ) );
				}
			}
		}

		return chainable ?
			elems :

			// Gets
			bulk ?
				fn.call( elems ) :
				length ? fn( elems[0], key ) : emptyGet;
	};
jQuery.active = 0;
jQuery.ajax = function( url, options ) {
/// <summary>
///     Perform an asynchronous HTTP (Ajax) request.
///     &#10;1 - jQuery.ajax(url, settings) 
///     &#10;2 - jQuery.ajax(settings)
/// </summary>
/// <param name="url" type="String">
///     A string containing the URL to which the request is sent.
/// </param>
/// <param name="options" type="Object">
///     A set of key/value pairs that configure the Ajax request. All settings are optional. A default can be set for any option with $.ajaxSetup(). See jQuery.ajax( settings ) below for a complete list of all settings.
/// </param>


		// If url is an object, simulate pre-1.5 signature
		if ( typeof url === "object" ) {
			options = url;
			url = undefined;
		}

		// Force options to be an object
		options = options || {};

		var // Cross-domain detection vars
			parts,
			// Loop variable
			i,
			// URL without anti-cache param
			cacheURL,
			// Response headers as string
			responseHeadersString,
			// timeout handle
			timeoutTimer,

			// To know if global events are to be dispatched
			fireGlobals,

			transport,
			// Response headers
			responseHeaders,
			// Create the final options object
			s = jQuery.ajaxSetup( {}, options ),
			// Callbacks context
			callbackContext = s.context || s,
			// Context for global events is callbackContext if it is a DOM node or jQuery collection
			globalEventContext = s.context && ( callbackContext.nodeType || callbackContext.jquery ) ?
				jQuery( callbackContext ) :
				jQuery.event,
			// Deferreds
			deferred = jQuery.Deferred(),
			completeDeferred = jQuery.Callbacks("once memory"),
			// Status-dependent callbacks
			statusCode = s.statusCode || {},
			// Headers (they are sent all at once)
			requestHeaders = {},
			requestHeadersNames = {},
			// The jqXHR state
			state = 0,
			// Default abort message
			strAbort = "canceled",
			// Fake xhr
			jqXHR = {
				readyState: 0,

				// Builds headers hashtable if needed
				getResponseHeader: function( key ) {
					var match;
					if ( state === 2 ) {
						if ( !responseHeaders ) {
							responseHeaders = {};
							while ( (match = rheaders.exec( responseHeadersString )) ) {
								responseHeaders[ match[1].toLowerCase() ] = match[ 2 ];
							}
						}
						match = responseHeaders[ key.toLowerCase() ];
					}
					return match == null ? null : match;
				},

				// Raw string
				getAllResponseHeaders: function() {
					return state === 2 ? responseHeadersString : null;
				},

				// Caches the header
				setRequestHeader: function( name, value ) {
					var lname = name.toLowerCase();
					if ( !state ) {
						name = requestHeadersNames[ lname ] = requestHeadersNames[ lname ] || name;
						requestHeaders[ name ] = value;
					}
					return this;
				},

				// Overrides response content-type header
				overrideMimeType: function( type ) {
					if ( !state ) {
						s.mimeType = type;
					}
					return this;
				},

				// Status-dependent callbacks
				statusCode: function( map ) {
					var code;
					if ( map ) {
						if ( state < 2 ) {
							for ( code in map ) {
								// Lazy-add the new callback in a way that preserves old ones
								statusCode[ code ] = [ statusCode[ code ], map[ code ] ];
							}
						} else {
							// Execute the appropriate callbacks
							jqXHR.always( map[ jqXHR.status ] );
						}
					}
					return this;
				},

				// Cancel the request
				abort: function( statusText ) {
					var finalText = statusText || strAbort;
					if ( transport ) {
						transport.abort( finalText );
					}
					done( 0, finalText );
					return this;
				}
			};

		// Attach deferreds
		deferred.promise( jqXHR ).complete = completeDeferred.add;
		jqXHR.success = jqXHR.done;
		jqXHR.error = jqXHR.fail;

		// Remove hash character (#7531: and string promotion)
		// Add protocol if not provided (#5866: IE7 issue with protocol-less urls)
		// Handle falsy url in the settings object (#10093: consistency with old signature)
		// We also use the url parameter if available
		s.url = ( ( url || s.url || ajaxLocation ) + "" ).replace( rhash, "" ).replace( rprotocol, ajaxLocParts[ 1 ] + "//" );

		// Alias method option to type as per ticket #12004
		s.type = options.method || options.type || s.method || s.type;

		// Extract dataTypes list
		s.dataTypes = jQuery.trim( s.dataType || "*" ).toLowerCase().match( core_rnotwhite ) || [""];

		// A cross-domain request is in order when we have a protocol:host:port mismatch
		if ( s.crossDomain == null ) {
			parts = rurl.exec( s.url.toLowerCase() );
			s.crossDomain = !!( parts &&
				( parts[ 1 ] !== ajaxLocParts[ 1 ] || parts[ 2 ] !== ajaxLocParts[ 2 ] ||
					( parts[ 3 ] || ( parts[ 1 ] === "http:" ? "80" : "443" ) ) !==
						( ajaxLocParts[ 3 ] || ( ajaxLocParts[ 1 ] === "http:" ? "80" : "443" ) ) )
			);
		}

		// Convert data if not already a string
		if ( s.data && s.processData && typeof s.data !== "string" ) {
			s.data = jQuery.param( s.data, s.traditional );
		}

		// Apply prefilters
		inspectPrefiltersOrTransports( prefilters, s, options, jqXHR );

		// If request was aborted inside a prefilter, stop there
		if ( state === 2 ) {
			return jqXHR;
		}

		// We can fire global events as of now if asked to
		fireGlobals = s.global;

		// Watch for a new set of requests
		if ( fireGlobals && jQuery.active++ === 0 ) {
			jQuery.event.trigger("ajaxStart");
		}

		// Uppercase the type
		s.type = s.type.toUpperCase();

		// Determine if request has content
		s.hasContent = !rnoContent.test( s.type );

		// Save the URL in case we're toying with the If-Modified-Since
		// and/or If-None-Match header later on
		cacheURL = s.url;

		// More options handling for requests with no content
		if ( !s.hasContent ) {

			// If data is available, append data to url
			if ( s.data ) {
				cacheURL = ( s.url += ( ajax_rquery.test( cacheURL ) ? "&" : "?" ) + s.data );
				// #9682: remove data so that it's not used in an eventual retry
				delete s.data;
			}

			// Add anti-cache in url if needed
			if ( s.cache === false ) {
				s.url = rts.test( cacheURL ) ?

					// If there is already a '_' parameter, set its value
					cacheURL.replace( rts, "$1_=" + ajax_nonce++ ) :

					// Otherwise add one to the end
					cacheURL + ( ajax_rquery.test( cacheURL ) ? "&" : "?" ) + "_=" + ajax_nonce++;
			}
		}

		// Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
		if ( s.ifModified ) {
			if ( jQuery.lastModified[ cacheURL ] ) {
				jqXHR.setRequestHeader( "If-Modified-Since", jQuery.lastModified[ cacheURL ] );
			}
			if ( jQuery.etag[ cacheURL ] ) {
				jqXHR.setRequestHeader( "If-None-Match", jQuery.etag[ cacheURL ] );
			}
		}

		// Set the correct header, if data is being sent
		if ( s.data && s.hasContent && s.contentType !== false || options.contentType ) {
			jqXHR.setRequestHeader( "Content-Type", s.contentType );
		}

		// Set the Accepts header for the server, depending on the dataType
		jqXHR.setRequestHeader(
			"Accept",
			s.dataTypes[ 0 ] && s.accepts[ s.dataTypes[0] ] ?
				s.accepts[ s.dataTypes[0] ] + ( s.dataTypes[ 0 ] !== "*" ? ", " + allTypes + "; q=0.01" : "" ) :
				s.accepts[ "*" ]
		);

		// Check for headers option
		for ( i in s.headers ) {
			jqXHR.setRequestHeader( i, s.headers[ i ] );
		}

		// Allow custom headers/mimetypes and early abort
		if ( s.beforeSend && ( s.beforeSend.call( callbackContext, jqXHR, s ) === false || state === 2 ) ) {
			// Abort if not done already and return
			return jqXHR.abort();
		}

		// aborting is no longer a cancellation
		strAbort = "abort";

		// Install callbacks on deferreds
		for ( i in { success: 1, error: 1, complete: 1 } ) {
			jqXHR[ i ]( s[ i ] );
		}

		// Get transport
		transport = inspectPrefiltersOrTransports( transports, s, options, jqXHR );

		// If no transport, we auto-abort
		if ( !transport ) {
			done( -1, "No Transport" );
		} else {
			jqXHR.readyState = 1;

			// Send global event
			if ( fireGlobals ) {
				globalEventContext.trigger( "ajaxSend", [ jqXHR, s ] );
			}
			// Timeout
			if ( s.async && s.timeout > 0 ) {
				timeoutTimer = setTimeout(function() {
					jqXHR.abort("timeout");
				}, s.timeout );
			}

			try {
				state = 1;
				transport.send( requestHeaders, done );
			} catch ( e ) {
				// Propagate exception as error if not done
				if ( state < 2 ) {
					done( -1, e );
				// Simply rethrow otherwise
				} else {
					throw e;
				}
			}
		}

		// Callback for when everything is done
		function done( status, nativeStatusText, responses, headers ) {
			var isSuccess, success, error, response, modified,
				statusText = nativeStatusText;

			// Called once
			if ( state === 2 ) {
				return;
			}

			// State is "done" now
			state = 2;

			// Clear timeout if it exists
			if ( timeoutTimer ) {
				clearTimeout( timeoutTimer );
			}

			// Dereference transport for early garbage collection
			// (no matter how long the jqXHR object will be used)
			transport = undefined;

			// Cache response headers
			responseHeadersString = headers || "";

			// Set readyState
			jqXHR.readyState = status > 0 ? 4 : 0;

			// Determine if successful
			isSuccess = status >= 200 && status < 300 || status === 304;

			// Get response data
			if ( responses ) {
				response = ajaxHandleResponses( s, jqXHR, responses );
			}

			// Convert no matter what (that way responseXXX fields are always set)
			response = ajaxConvert( s, response, jqXHR, isSuccess );

			// If successful, handle type chaining
			if ( isSuccess ) {

				// Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
				if ( s.ifModified ) {
					modified = jqXHR.getResponseHeader("Last-Modified");
					if ( modified ) {
						jQuery.lastModified[ cacheURL ] = modified;
					}
					modified = jqXHR.getResponseHeader("etag");
					if ( modified ) {
						jQuery.etag[ cacheURL ] = modified;
					}
				}

				// if no content
				if ( status === 204 || s.type === "HEAD" ) {
					statusText = "nocontent";

				// if not modified
				} else if ( status === 304 ) {
					statusText = "notmodified";

				// If we have data, let's convert it
				} else {
					statusText = response.state;
					success = response.data;
					error = response.error;
					isSuccess = !error;
				}
			} else {
				// We extract error from statusText
				// then normalize statusText and status for non-aborts
				error = statusText;
				if ( status || !statusText ) {
					statusText = "error";
					if ( status < 0 ) {
						status = 0;
					}
				}
			}

			// Set data for the fake xhr object
			jqXHR.status = status;
			jqXHR.statusText = ( nativeStatusText || statusText ) + "";

			// Success/Error
			if ( isSuccess ) {
				deferred.resolveWith( callbackContext, [ success, statusText, jqXHR ] );
			} else {
				deferred.rejectWith( callbackContext, [ jqXHR, statusText, error ] );
			}

			// Status-dependent callbacks
			jqXHR.statusCode( statusCode );
			statusCode = undefined;

			if ( fireGlobals ) {
				globalEventContext.trigger( isSuccess ? "ajaxSuccess" : "ajaxError",
					[ jqXHR, s, isSuccess ? success : error ] );
			}

			// Complete
			completeDeferred.fireWith( callbackContext, [ jqXHR, statusText ] );

			if ( fireGlobals ) {
				globalEventContext.trigger( "ajaxComplete", [ jqXHR, s ] );
				// Handle the global AJAX counter
				if ( !( --jQuery.active ) ) {
					jQuery.event.trigger("ajaxStop");
				}
			}
		}

		return jqXHR;
	};
jQuery.ajaxPrefilter = function( dataTypeExpression, func ) {
/// <summary>
///     Handle custom Ajax options or modify existing options before each request is sent and before they are processed by $.ajax().
/// </summary>
/// <param name="dataTypeExpression" type="String">
///     An optional string containing one or more space-separated dataTypes
/// </param>
/// <param name="func" type="Function">
///     A handler to set default values for future Ajax requests.
/// </param>
/// <returns type="undefined" />


		if ( typeof dataTypeExpression !== "string" ) {
			func = dataTypeExpression;
			dataTypeExpression = "*";
		}

		var dataType,
			i = 0,
			dataTypes = dataTypeExpression.toLowerCase().match( core_rnotwhite ) || [];

		if ( jQuery.isFunction( func ) ) {
			// For each dataType in the dataTypeExpression
			while ( (dataType = dataTypes[i++]) ) {
				// Prepend if requested
				if ( dataType[0] === "+" ) {
					dataType = dataType.slice( 1 ) || "*";
					(structure[ dataType ] = structure[ dataType ] || []).unshift( func );

				// Otherwise append
				} else {
					(structure[ dataType ] = structure[ dataType ] || []).push( func );
				}
			}
		}
	};
jQuery.ajaxSettings = { "url": 'http://jqueryvsdocgen.azurewebsites.net/?ver=1.10.2&newLineMethod=xml',
"type": 'GET',
"isLocal": false,
"global": true,
"processData": true,
"async": true,
"contentType": 'application/x-www-form-urlencoded; charset=UTF-8',
"accepts": {},
"contents": {},
"responseFields": {},
"converters": {},
"flatOptions": {},
"jsonp": 'callback' };
jQuery.ajaxSetup = function( target, settings ) {
/// <summary>
///     Set default values for future Ajax requests.
/// </summary>
/// <param name="target" type="Object">
///     A set of key/value pairs that configure the default Ajax request. All options are optional.
/// </param>

		return settings ?

			// Building a settings object
			ajaxExtend( ajaxExtend( target, jQuery.ajaxSettings ), settings ) :

			// Extending ajaxSettings
			ajaxExtend( jQuery.ajaxSettings, target );
	};
jQuery.ajaxTransport = function( dataTypeExpression, func ) {


		if ( typeof dataTypeExpression !== "string" ) {
			func = dataTypeExpression;
			dataTypeExpression = "*";
		}

		var dataType,
			i = 0,
			dataTypes = dataTypeExpression.toLowerCase().match( core_rnotwhite ) || [];

		if ( jQuery.isFunction( func ) ) {
			// For each dataType in the dataTypeExpression
			while ( (dataType = dataTypes[i++]) ) {
				// Prepend if requested
				if ( dataType[0] === "+" ) {
					dataType = dataType.slice( 1 ) || "*";
					(structure[ dataType ] = structure[ dataType ] || []).unshift( func );

				// Otherwise append
				} else {
					(structure[ dataType ] = structure[ dataType ] || []).push( func );
				}
			}
		}
	};
jQuery.attr = function( elem, name, value ) {

		var hooks, ret,
			nType = elem.nodeType;

		// don't get/set attributes on text, comment and attribute nodes
		if ( !elem || nType === 3 || nType === 8 || nType === 2 ) {
			return;
		}

		// Fallback to prop when attributes are not supported
		if ( typeof elem.getAttribute === core_strundefined ) {
			return jQuery.prop( elem, name, value );
		}

		// All attributes are lowercase
		// Grab necessary hook if one is defined
		if ( nType !== 1 || !jQuery.isXMLDoc( elem ) ) {
			name = name.toLowerCase();
			hooks = jQuery.attrHooks[ name ] ||
				( jQuery.expr.match.bool.test( name ) ? boolHook : nodeHook );
		}

		if ( value !== undefined ) {

			if ( value === null ) {
				jQuery.removeAttr( elem, name );

			} else if ( hooks && "set" in hooks && (ret = hooks.set( elem, value, name )) !== undefined ) {
				return ret;

			} else {
				elem.setAttribute( name, value + "" );
				return value;
			}

		} else if ( hooks && "get" in hooks && (ret = hooks.get( elem, name )) !== null ) {
			return ret;

		} else {
			ret = jQuery.find.attr( elem, name );

			// Non-existent attributes return null, we normalize to undefined
			return ret == null ?
				undefined :
				ret;
		}
	};
jQuery.attrHooks = { "type": {} };
jQuery.buildFragment = function( elems, context, scripts, selection ) {

		var j, elem, contains,
			tmp, tag, tbody, wrap,
			l = elems.length,

			// Ensure a safe fragment
			safe = createSafeFragment( context ),

			nodes = [],
			i = 0;

		for ( ; i < l; i++ ) {
			elem = elems[ i ];

			if ( elem || elem === 0 ) {

				// Add nodes directly
				if ( jQuery.type( elem ) === "object" ) {
					jQuery.merge( nodes, elem.nodeType ? [ elem ] : elem );

				// Convert non-html into a text node
				} else if ( !rhtml.test( elem ) ) {
					nodes.push( context.createTextNode( elem ) );

				// Convert html into DOM nodes
				} else {
					tmp = tmp || safe.appendChild( context.createElement("div") );

					// Deserialize a standard representation
					tag = ( rtagName.exec( elem ) || ["", ""] )[1].toLowerCase();
					wrap = wrapMap[ tag ] || wrapMap._default;

					tmp.innerHTML = wrap[1] + elem.replace( rxhtmlTag, "<$1></$2>" ) + wrap[2];

					// Descend through wrappers to the right content
					j = wrap[0];
					while ( j-- ) {
						tmp = tmp.lastChild;
					}

					// Manually add leading whitespace removed by IE
					if ( !jQuery.support.leadingWhitespace && rleadingWhitespace.test( elem ) ) {
						nodes.push( context.createTextNode( rleadingWhitespace.exec( elem )[0] ) );
					}

					// Remove IE's autoinserted <tbody> from table fragments
					if ( !jQuery.support.tbody ) {

						// String was a <table>, *may* have spurious <tbody>
						elem = tag === "table" && !rtbody.test( elem ) ?
							tmp.firstChild :

							// String was a bare <thead> or <tfoot>
							wrap[1] === "<table>" && !rtbody.test( elem ) ?
								tmp :
								0;

						j = elem && elem.childNodes.length;
						while ( j-- ) {
							if ( jQuery.nodeName( (tbody = elem.childNodes[j]), "tbody" ) && !tbody.childNodes.length ) {
								elem.removeChild( tbody );
							}
						}
					}

					jQuery.merge( nodes, tmp.childNodes );

					// Fix #12392 for WebKit and IE > 9
					tmp.textContent = "";

					// Fix #12392 for oldIE
					while ( tmp.firstChild ) {
						tmp.removeChild( tmp.firstChild );
					}

					// Remember the top-level container for proper cleanup
					tmp = safe.lastChild;
				}
			}
		}

		// Fix #11356: Clear elements from fragment
		if ( tmp ) {
			safe.removeChild( tmp );
		}

		// Reset defaultChecked for any radios and checkboxes
		// about to be appended to the DOM in IE 6/7 (#8060)
		if ( !jQuery.support.appendChecked ) {
			jQuery.grep( getAll( nodes, "input" ), fixDefaultChecked );
		}

		i = 0;
		while ( (elem = nodes[ i++ ]) ) {

			// #4087 - If origin and destination elements are the same, and this is
			// that element, do not do anything
			if ( selection && jQuery.inArray( elem, selection ) !== -1 ) {
				continue;
			}

			contains = jQuery.contains( elem.ownerDocument, elem );

			// Append to fragment
			tmp = getAll( safe.appendChild( elem ), "script" );

			// Preserve script evaluation history
			if ( contains ) {
				setGlobalEval( tmp );
			}

			// Capture executables
			if ( scripts ) {
				j = 0;
				while ( (elem = tmp[ j++ ]) ) {
					if ( rscriptType.test( elem.type || "" ) ) {
						scripts.push( elem );
					}
				}
			}
		}

		tmp = null;

		return safe;
	};
jQuery.cache = {};
jQuery.camelCase = function( string ) {

		return string.replace( rmsPrefix, "ms-" ).replace( rdashAlpha, fcamelCase );
	};
jQuery.cleanData = function( elems, /* internal */ acceptData ) {

		var elem, type, id, data,
			i = 0,
			internalKey = jQuery.expando,
			cache = jQuery.cache,
			deleteExpando = jQuery.support.deleteExpando,
			special = jQuery.event.special;

		for ( ; (elem = elems[i]) != null; i++ ) {

			if ( acceptData || jQuery.acceptData( elem ) ) {

				id = elem[ internalKey ];
				data = id && cache[ id ];

				if ( data ) {
					if ( data.events ) {
						for ( type in data.events ) {
							if ( special[ type ] ) {
								jQuery.event.remove( elem, type );

							// This is a shortcut to avoid jQuery.event.remove's overhead
							} else {
								jQuery.removeEvent( elem, type, data.handle );
							}
						}
					}

					// Remove cache only if it was not already removed by jQuery.event.remove
					if ( cache[ id ] ) {

						delete cache[ id ];

						// IE does not allow us to delete expando properties from nodes,
						// nor does it have a removeAttribute function on Document nodes;
						// we must handle all of these cases
						if ( deleteExpando ) {
							delete elem[ internalKey ];

						} else if ( typeof elem.removeAttribute !== core_strundefined ) {
							elem.removeAttribute( internalKey );

						} else {
							elem[ internalKey ] = null;
						}

						core_deletedIds.push( id );
					}
				}
			}
		}
	};
jQuery.clone = function( elem, dataAndEvents, deepDataAndEvents ) {

		var destElements, node, clone, i, srcElements,
			inPage = jQuery.contains( elem.ownerDocument, elem );

		if ( jQuery.support.html5Clone || jQuery.isXMLDoc(elem) || !rnoshimcache.test( "<" + elem.nodeName + ">" ) ) {
			clone = elem.cloneNode( true );

		// IE<=8 does not properly clone detached, unknown element nodes
		} else {
			fragmentDiv.innerHTML = elem.outerHTML;
			fragmentDiv.removeChild( clone = fragmentDiv.firstChild );
		}

		if ( (!jQuery.support.noCloneEvent || !jQuery.support.noCloneChecked) &&
				(elem.nodeType === 1 || elem.nodeType === 11) && !jQuery.isXMLDoc(elem) ) {

			// We eschew Sizzle here for performance reasons: http://jsperf.com/getall-vs-sizzle/2
			destElements = getAll( clone );
			srcElements = getAll( elem );

			// Fix all IE cloning issues
			for ( i = 0; (node = srcElements[i]) != null; ++i ) {
				// Ensure that the destination node is not null; Fixes #9587
				if ( destElements[i] ) {
					fixCloneNodeIssues( node, destElements[i] );
				}
			}
		}

		// Copy the events from the original to the clone
		if ( dataAndEvents ) {
			if ( deepDataAndEvents ) {
				srcElements = srcElements || getAll( elem );
				destElements = destElements || getAll( clone );

				for ( i = 0; (node = srcElements[i]) != null; i++ ) {
					cloneCopyEvent( node, destElements[i] );
				}
			} else {
				cloneCopyEvent( elem, clone );
			}
		}

		// Preserve script evaluation history
		destElements = getAll( clone, "script" );
		if ( destElements.length > 0 ) {
			setGlobalEval( destElements, !inPage && getAll( elem, "script" ) );
		}

		destElements = srcElements = node = null;

		// Return the cloned set
		return clone;
	};
jQuery.contains = function( context, elem ) {
/// <summary>
///     Check to see if a DOM element is within another DOM element.
/// </summary>
/// <param name="context" domElement="true">
///     The DOM element that may contain the other element.
/// </param>
/// <param name="elem" domElement="true">
///     The DOM element that may be contained by the other element.
/// </param>
/// <returns type="Boolean" />

	// Set document vars if needed
	if ( ( context.ownerDocument || context ) !== document ) {
		setDocument( context );
	}
	return contains( context, elem );
};
jQuery.css = function( elem, name, extra, styles ) {

		var num, val, hooks,
			origName = jQuery.camelCase( name );

		// Make sure that we're working with the right name
		name = jQuery.cssProps[ origName ] || ( jQuery.cssProps[ origName ] = vendorPropName( elem.style, origName ) );

		// gets hook for the prefixed version
		// followed by the unprefixed version
		hooks = jQuery.cssHooks[ name ] || jQuery.cssHooks[ origName ];

		// If a hook was provided get the computed value from there
		if ( hooks && "get" in hooks ) {
			val = hooks.get( elem, true, extra );
		}

		// Otherwise, if a way to get the computed value exists, use that
		if ( val === undefined ) {
			val = curCSS( elem, name, styles );
		}

		//convert "normal" to computed value
		if ( val === "normal" && name in cssNormalTransform ) {
			val = cssNormalTransform[ name ];
		}

		// Return, converting to number if forced or a qualifier was provided and val looks numeric
		if ( extra === "" || extra ) {
			num = parseFloat( val );
			return extra === true || jQuery.isNumeric( num ) ? num || 0 : val;
		}
		return val;
	};
jQuery.cssHooks = { "opacity": {},
"height": {},
"width": {},
"margin": {},
"padding": {},
"borderWidth": {} };
jQuery.cssNumber = { "columnCount": true,
"fillOpacity": true,
"fontWeight": true,
"lineHeight": true,
"opacity": true,
"orphans": true,
"widows": true,
"zIndex": true,
"zoom": true };
jQuery.cssProps = { "float": 'cssFloat',
"display": 'display',
"visibility": 'visibility' };
jQuery.data = function( elem, name, data ) {
/// <summary>
///     1: Store arbitrary data associated with the specified element. Returns the value that was set.
///     &#10;    1.1 - jQuery.data(element, key, value)
///     &#10;2: Returns value at named data store for the element, as set by jQuery.data(element, name, value), or the full data store for the element.
///     &#10;    2.1 - jQuery.data(element, key) 
///     &#10;    2.2 - jQuery.data(element)
/// </summary>
/// <param name="elem" domElement="true">
///     The DOM element to associate with the data.
/// </param>
/// <param name="name" type="String">
///     A string naming the piece of data to set.
/// </param>
/// <param name="data" type="Object">
///     The new data value.
/// </param>
/// <returns type="Object" />

		return internalData( elem, name, data );
	};
jQuery.dequeue = function( elem, type ) {
/// <summary>
///     Execute the next function on the queue for the matched element.
/// </summary>
/// <param name="elem" domElement="true">
///     A DOM element from which to remove and execute a queued function.
/// </param>
/// <param name="type" type="String">
///     A string containing the name of the queue. Defaults to fx, the standard effects queue.
/// </param>
/// <returns type="undefined" />

		type = type || "fx";

		var queue = jQuery.queue( elem, type ),
			startLength = queue.length,
			fn = queue.shift(),
			hooks = jQuery._queueHooks( elem, type ),
			next = function() {
				jQuery.dequeue( elem, type );
			};

		// If the fx queue is dequeued, always remove the progress sentinel
		if ( fn === "inprogress" ) {
			fn = queue.shift();
			startLength--;
		}

		hooks.cur = fn;
		if ( fn ) {

			// Add a progress sentinel to prevent the fx queue from being
			// automatically dequeued
			if ( type === "fx" ) {
				queue.unshift( "inprogress" );
			}

			// clear up the last queue stop function
			delete hooks.stop;
			fn.call( elem, next, hooks );
		}

		if ( !startLength && hooks ) {
			hooks.empty.fire();
		}
	};
jQuery.dir = function( elem, dir, until ) {

		var matched = [],
			cur = elem[ dir ];

		while ( cur && cur.nodeType !== 9 && (until === undefined || cur.nodeType !== 1 || !jQuery( cur ).is( until )) ) {
			if ( cur.nodeType === 1 ) {
				matched.push( cur );
			}
			cur = cur[dir];
		}
		return matched;
	};
jQuery.each = function( obj, callback, args ) {
/// <summary>
///     A generic iterator function, which can be used to seamlessly iterate over both objects and arrays. Arrays and array-like objects with a length property (such as a function's arguments object) are iterated by numeric index, from 0 to length-1. Other objects are iterated via their named properties.
/// </summary>
/// <param name="obj" type="Object">
///     The object or array to iterate over.
/// </param>
/// <param name="callback" type="Function">
///     The function that will be executed on every object.
/// </param>
/// <returns type="Object" />

		var value,
			i = 0,
			length = obj.length,
			isArray = isArraylike( obj );

		if ( args ) {
			if ( isArray ) {
				for ( ; i < length; i++ ) {
					value = callback.apply( obj[ i ], args );

					if ( value === false ) {
						break;
					}
				}
			} else {
				for ( i in obj ) {
					value = callback.apply( obj[ i ], args );

					if ( value === false ) {
						break;
					}
				}
			}

		// A special, fast, case for the most common use of each
		} else {
			if ( isArray ) {
				for ( ; i < length; i++ ) {
					value = callback.call( obj[ i ], i, obj[ i ] );

					if ( value === false ) {
						break;
					}
				}
			} else {
				for ( i in obj ) {
					value = callback.call( obj[ i ], i, obj[ i ] );

					if ( value === false ) {
						break;
					}
				}
			}
		}

		return obj;
	};
jQuery.easing = {};
jQuery.error = function( msg ) {
/// <summary>
///     Takes a string and throws an exception containing it.
/// </summary>
/// <param name="msg" type="String">
///     The message to send out.
/// </param>

		throw new Error( msg );
	};
jQuery.etag = {};
jQuery.event = { "global": {},
"props": ['altKey','bubbles','cancelable','ctrlKey','currentTarget','eventPhase','metaKey','relatedTarget','shiftKey','target','timeStamp','view','which'],
"fixHooks": {},
"keyHooks": {},
"mouseHooks": {},
"special": {},
"triggered": {} };
jQuery.expr = { "cacheLength": 50,
"match": {},
"attrHandle": {},
"find": {},
"relative": {},
"preFilter": {},
"filter": {},
"pseudos": {},
"filters": {},
"setFilters": {},
":": {} };
jQuery.extend = function() {
/// <summary>
///     Merge the contents of two or more objects together into the first object.
///     &#10;1 - jQuery.extend(target, object1, objectN) 
///     &#10;2 - jQuery.extend(deep, target, object1, objectN)
/// </summary>
/// <param name="" type="Boolean">
///     If true, the merge becomes recursive (aka. deep copy).
/// </param>
/// <param name="" type="Object">
///     The object to extend. It will receive the new properties.
/// </param>
/// <param name="" type="Object">
///     An object containing additional properties to merge in.
/// </param>
/// <param name="" type="Object">
///     Additional objects containing properties to merge in.
/// </param>
/// <returns type="Object" />

	var src, copyIsArray, copy, name, options, clone,
		target = arguments[0] || {},
		i = 1,
		length = arguments.length,
		deep = false;

	// Handle a deep copy situation
	if ( typeof target === "boolean" ) {
		deep = target;
		target = arguments[1] || {};
		// skip the boolean and the target
		i = 2;
	}

	// Handle case when target is a string or something (possible in deep copy)
	if ( typeof target !== "object" && !jQuery.isFunction(target) ) {
		target = {};
	}

	// extend jQuery itself if only one argument is passed
	if ( length === i ) {
		target = this;
		--i;
	}

	for ( ; i < length; i++ ) {
		// Only deal with non-null/undefined values
		if ( (options = arguments[ i ]) != null ) {
			// Extend the base object
			for ( name in options ) {
				src = target[ name ];
				copy = options[ name ];

				// Prevent never-ending loop
				if ( target === copy ) {
					continue;
				}

				// Recurse if we're merging plain objects or arrays
				if ( deep && copy && ( jQuery.isPlainObject(copy) || (copyIsArray = jQuery.isArray(copy)) ) ) {
					if ( copyIsArray ) {
						copyIsArray = false;
						clone = src && jQuery.isArray(src) ? src : [];

					} else {
						clone = src && jQuery.isPlainObject(src) ? src : {};
					}

					// Never move original objects, clone them
					target[ name ] = jQuery.extend( deep, clone, copy );

				// Don't bring in undefined values
				} else if ( copy !== undefined ) {
					target[ name ] = copy;
				}
			}
		}
	}

	// Return the modified object
	return target;
};
jQuery.filter = function( expr, elems, not ) {

		var elem = elems[ 0 ];

		if ( not ) {
			expr = ":not(" + expr + ")";
		}

		return elems.length === 1 && elem.nodeType === 1 ?
			jQuery.find.matchesSelector( elem, expr ) ? [ elem ] : [] :
			jQuery.find.matches( expr, jQuery.grep( elems, function( elem ) {
				return elem.nodeType === 1;
			}));
	};
jQuery.find = function Sizzle( selector, context, results, seed ) {

	var match, elem, m, nodeType,
		// QSA vars
		i, groups, old, nid, newContext, newSelector;

	if ( ( context ? context.ownerDocument || context : preferredDoc ) !== document ) {
		setDocument( context );
	}

	context = context || document;
	results = results || [];

	if ( !selector || typeof selector !== "string" ) {
		return results;
	}

	if ( (nodeType = context.nodeType) !== 1 && nodeType !== 9 ) {
		return [];
	}

	if ( documentIsHTML && !seed ) {

		// Shortcuts
		if ( (match = rquickExpr.exec( selector )) ) {
			// Speed-up: Sizzle("#ID")
			if ( (m = match[1]) ) {
				if ( nodeType === 9 ) {
					elem = context.getElementById( m );
					// Check parentNode to catch when Blackberry 4.6 returns
					// nodes that are no longer in the document #6963
					if ( elem && elem.parentNode ) {
						// Handle the case where IE, Opera, and Webkit return items
						// by name instead of ID
						if ( elem.id === m ) {
							results.push( elem );
							return results;
						}
					} else {
						return results;
					}
				} else {
					// Context is not a document
					if ( context.ownerDocument && (elem = context.ownerDocument.getElementById( m )) &&
						contains( context, elem ) && elem.id === m ) {
						results.push( elem );
						return results;
					}
				}

			// Speed-up: Sizzle("TAG")
			} else if ( match[2] ) {
				push.apply( results, context.getElementsByTagName( selector ) );
				return results;

			// Speed-up: Sizzle(".CLASS")
			} else if ( (m = match[3]) && support.getElementsByClassName && context.getElementsByClassName ) {
				push.apply( results, context.getElementsByClassName( m ) );
				return results;
			}
		}

		// QSA path
		if ( support.qsa && (!rbuggyQSA || !rbuggyQSA.test( selector )) ) {
			nid = old = expando;
			newContext = context;
			newSelector = nodeType === 9 && selector;

			// qSA works strangely on Element-rooted queries
			// We can work around this by specifying an extra ID on the root
			// and working up from there (Thanks to Andrew Dupont for the technique)
			// IE 8 doesn't work on object elements
			if ( nodeType === 1 && context.nodeName.toLowerCase() !== "object" ) {
				groups = tokenize( selector );

				if ( (old = context.getAttribute("id")) ) {
					nid = old.replace( rescape, "\\$&" );
				} else {
					context.setAttribute( "id", nid );
				}
				nid = "[id='" + nid + "'] ";

				i = groups.length;
				while ( i-- ) {
					groups[i] = nid + toSelector( groups[i] );
				}
				newContext = rsibling.test( selector ) && context.parentNode || context;
				newSelector = groups.join(",");
			}

			if ( newSelector ) {
				try {
					push.apply( results,
						newContext.querySelectorAll( newSelector )
					);
					return results;
				} catch(qsaError) {
				} finally {
					if ( !old ) {
						context.removeAttribute("id");
					}
				}
			}
		}
	}

	// All others
	return select( selector.replace( rtrim, "$1" ), context, results, seed );
};
jQuery.fn = { "jquery": '1.10.2',
"selector": '',
"length": 0 };
jQuery.fx = function( elem, options, prop, end, easing, unit ) {

		this.elem = elem;
		this.prop = prop;
		this.easing = easing || "swing";
		this.options = options;
		this.start = this.now = this.cur();
		this.end = end;
		this.unit = unit || ( jQuery.cssNumber[ prop ] ? "" : "px" );
	};
jQuery.get = function( url, data, callback, type ) {
/// <summary>
///     Load data from the server using a HTTP GET request.
/// </summary>
/// <param name="url" type="String">
///     A string containing the URL to which the request is sent.
/// </param>
/// <param name="data" type="String">
///     A map or string that is sent to the server with the request.
/// </param>
/// <param name="callback" type="Function">
///     A callback function that is executed if the request succeeds.
/// </param>
/// <param name="type" type="String">
///     The type of data expected from the server. Default: Intelligent Guess (xml, json, script, or html).
/// </param>

		// shift arguments if data argument was omitted
		if ( jQuery.isFunction( data ) ) {
			type = type || callback;
			callback = data;
			data = undefined;
		}

		return jQuery.ajax({
			url: url,
			type: method,
			dataType: type,
			data: data,
			success: callback
		});
	};
jQuery.getJSON = function( url, data, callback ) {
/// <summary>
///     Load JSON-encoded data from the server using a GET HTTP request.
/// </summary>
/// <param name="url" type="String">
///     A string containing the URL to which the request is sent.
/// </param>
/// <param name="data" type="Object">
///     A map or string that is sent to the server with the request.
/// </param>
/// <param name="callback" type="Function">
///     A callback function that is executed if the request succeeds.
/// </param>

		return jQuery.get( url, data, callback, "json" );
	};
jQuery.getScript = function( url, callback ) {
/// <summary>
///     Load a JavaScript file from the server using a GET HTTP request, then execute it.
/// </summary>
/// <param name="url" type="String">
///     A string containing the URL to which the request is sent.
/// </param>
/// <param name="callback" type="Function">
///     A callback function that is executed if the request succeeds.
/// </param>

		return jQuery.get( url, undefined, callback, "script" );
	};
jQuery.globalEval = function( data ) {
/// <summary>
///     Execute some JavaScript code globally.
/// </summary>
/// <param name="data" type="String">
///     The JavaScript code to execute.
/// </param>

		if ( data && jQuery.trim( data ) ) {
			// We use execScript on Internet Explorer
			// We use an anonymous function so that context is window
			// rather than jQuery in Firefox
			( window.execScript || function( data ) {
				window[ "eval" ].call( window, data );
			} )( data );
		}
	};
jQuery.grep = function( elems, callback, inv ) {
/// <summary>
///     Finds the elements of an array which satisfy a filter function. The original array is not affected.
/// </summary>
/// <param name="elems" type="Array">
///     The array to search through.
/// </param>
/// <param name="callback" type="Function">
///     The function to process each item against.  The first argument to the function is the item, and the second argument is the index.  The function should return a Boolean value.  this will be the global window object.
/// </param>
/// <param name="inv" type="Boolean">
///     If "invert" is false, or not provided, then the function returns an array consisting of all elements for which "callback" returns true.  If "invert" is true, then the function returns an array consisting of all elements for which "callback" returns false.
/// </param>
/// <returns type="Array" />

		var retVal,
			ret = [],
			i = 0,
			length = elems.length;
		inv = !!inv;

		// Go through the array, only saving the items
		// that pass the validator function
		for ( ; i < length; i++ ) {
			retVal = !!callback( elems[ i ], i );
			if ( inv !== retVal ) {
				ret.push( elems[ i ] );
			}
		}

		return ret;
	};
jQuery.guid = 1;
jQuery.hasData = function( elem ) {
/// <summary>
///     Determine whether an element has any jQuery data associated with it.
/// </summary>
/// <param name="elem" domElement="true">
///     A DOM element to be checked for data.
/// </param>
/// <returns type="Boolean" />

		elem = elem.nodeType ? jQuery.cache[ elem[jQuery.expando] ] : elem[ jQuery.expando ];
		return !!elem && !isEmptyDataObject( elem );
	};
jQuery.holdReady = function( hold ) {
/// <summary>
///     Holds or releases the execution of jQuery's ready event.
/// </summary>
/// <param name="hold" type="Boolean">
///     Indicates whether the ready hold is being requested or released
/// </param>
/// <returns type="undefined" />

		if ( hold ) {
			jQuery.readyWait++;
		} else {
			jQuery.ready( true );
		}
	};
jQuery.inArray = function( elem, arr, i ) {
/// <summary>
///     Search for a specified value within an array and return its index (or -1 if not found).
/// </summary>
/// <param name="elem" type="Object">
///     The value to search for.
/// </param>
/// <param name="arr" type="Array">
///     An array through which to search.
/// </param>
/// <param name="i" type="Number">
///     The index of the array at which to begin the search. The default is 0, which will search the whole array.
/// </param>
/// <returns type="Number" />

		var len;

		if ( arr ) {
			if ( core_indexOf ) {
				return core_indexOf.call( arr, elem, i );
			}

			len = arr.length;
			i = i ? i < 0 ? Math.max( 0, len + i ) : i : 0;

			for ( ; i < len; i++ ) {
				// Skip accessing in sparse arrays
				if ( i in arr && arr[ i ] === elem ) {
					return i;
				}
			}
		}

		return -1;
	};
jQuery.isEmptyObject = function( obj ) {
/// <summary>
///     Check to see if an object is empty (contains no properties).
/// </summary>
/// <param name="obj" type="Object">
///     The object that will be checked to see if it's empty.
/// </param>
/// <returns type="Boolean" />

		var name;
		for ( name in obj ) {
			return false;
		}
		return true;
	};
jQuery.isFunction = function( obj ) {
/// <summary>
///     Determine if the argument passed is a Javascript function object.
/// </summary>
/// <param name="obj" type="Object">
///     Object to test whether or not it is a function.
/// </param>
/// <returns type="boolean" />

		return jQuery.type(obj) === "function";
	};
jQuery.isNumeric = function( obj ) {
/// <summary>
///     Determines whether its argument is a number.
/// </summary>
/// <param name="obj" type="Object">
///     The value to be tested.
/// </param>
/// <returns type="Boolean" />

		return !isNaN( parseFloat(obj) ) && isFinite( obj );
	};
jQuery.isPlainObject = function( obj ) {
/// <summary>
///     Check to see if an object is a plain object (created using "{}" or "new Object").
/// </summary>
/// <param name="obj" type="Object">
///     The object that will be checked to see if it's a plain object.
/// </param>
/// <returns type="Boolean" />

		var key;

		// Must be an Object.
		// Because of IE, we also have to check the presence of the constructor property.
		// Make sure that DOM nodes and window objects don't pass through, as well
		if ( !obj || jQuery.type(obj) !== "object" || obj.nodeType || jQuery.isWindow( obj ) ) {
			return false;
		}

		try {
			// Not own constructor property must be Object
			if ( obj.constructor &&
				!core_hasOwn.call(obj, "constructor") &&
				!core_hasOwn.call(obj.constructor.prototype, "isPrototypeOf") ) {
				return false;
			}
		} catch ( e ) {
			// IE8,9 Will throw exceptions on certain host objects #9897
			return false;
		}

		// Support: IE<9
		// Handle iteration over inherited properties before own properties.
		if ( jQuery.support.ownLast ) {
			for ( key in obj ) {
				return core_hasOwn.call( obj, key );
			}
		}

		// Own properties are enumerated firstly, so to speed up,
		// if last one is own, then all properties are own.
		for ( key in obj ) {}

		return key === undefined || core_hasOwn.call( obj, key );
	};
jQuery.isReady = true;
jQuery.isWindow = function( obj ) {
/// <summary>
///     Determine whether the argument is a window.
/// </summary>
/// <param name="obj" type="Object">
///     Object to test whether or not it is a window.
/// </param>
/// <returns type="boolean" />

		/* jshint eqeqeq: false */
		return obj != null && obj == obj.window;
	};
jQuery.isXMLDoc = function( elem ) {
/// <summary>
///     Check to see if a DOM node is within an XML document (or is an XML document).
/// </summary>
/// <param name="elem" domElement="true">
///     The DOM node that will be checked to see if it's in an XML document.
/// </param>
/// <returns type="Boolean" />

	// documentElement is verified for cases where it doesn't yet exist
	// (such as loading iframes in IE - #4833)
	var documentElement = elem && (elem.ownerDocument || elem).documentElement;
	return documentElement ? documentElement.nodeName !== "HTML" : false;
};
jQuery.lastModified = {};
jQuery.makeArray = function( arr, results ) {
/// <summary>
///     Convert an array-like object into a true JavaScript array.
/// </summary>
/// <param name="arr" type="Object">
///     Any object to turn into a native Array.
/// </param>
/// <returns type="Array" />

		var ret = results || [];

		if ( arr != null ) {
			if ( isArraylike( Object(arr) ) ) {
				jQuery.merge( ret,
					typeof arr === "string" ?
					[ arr ] : arr
				);
			} else {
				core_push.call( ret, arr );
			}
		}

		return ret;
	};
jQuery.map = function( elems, callback, arg ) {
/// <summary>
///     Translate all items in an array or object to new array of items.
///     &#10;1 - jQuery.map(array, callback(elementOfArray, indexInArray)) 
///     &#10;2 - jQuery.map(arrayOrObject, callback( value, indexOrKey ))
/// </summary>
/// <param name="elems" type="Array">
///     The Array to translate.
/// </param>
/// <param name="callback" type="Function">
///     The function to process each item against.  The first argument to the function is the array item, the second argument is the index in array The function can return any value. Within the function, this refers to the global (window) object.
/// </param>
/// <returns type="Array" />

		var value,
			i = 0,
			length = elems.length,
			isArray = isArraylike( elems ),
			ret = [];

		// Go through the array, translating each of the items to their
		if ( isArray ) {
			for ( ; i < length; i++ ) {
				value = callback( elems[ i ], i, arg );

				if ( value != null ) {
					ret[ ret.length ] = value;
				}
			}

		// Go through every key on the object,
		} else {
			for ( i in elems ) {
				value = callback( elems[ i ], i, arg );

				if ( value != null ) {
					ret[ ret.length ] = value;
				}
			}
		}

		// Flatten any nested arrays
		return core_concat.apply( [], ret );
	};
jQuery.merge = function( first, second ) {
/// <summary>
///     Merge the contents of two arrays together into the first array.
/// </summary>
/// <param name="first" type="Array">
///     The first array to merge, the elements of second added.
/// </param>
/// <param name="second" type="Array">
///     The second array to merge into the first, unaltered.
/// </param>
/// <returns type="Array" />

		var l = second.length,
			i = first.length,
			j = 0;

		if ( typeof l === "number" ) {
			for ( ; j < l; j++ ) {
				first[ i++ ] = second[ j ];
			}
		} else {
			while ( second[j] !== undefined ) {
				first[ i++ ] = second[ j++ ];
			}
		}

		first.length = i;

		return first;
	};
jQuery.noConflict = function( deep ) {
/// <summary>
///     Relinquish jQuery's control of the $ variable.
/// </summary>
/// <param name="deep" type="Boolean">
///     A Boolean indicating whether to remove all jQuery variables from the global scope (including jQuery itself).
/// </param>
/// <returns type="Object" />

		if ( window.$ === jQuery ) {
			window.$ = _$;
		}

		if ( deep && window.jQuery === jQuery ) {
			window.jQuery = _jQuery;
		}

		return jQuery;
	};
jQuery.noData = { "applet": true,
"embed": true,
"object": 'clsid:D27CDB6E-AE6D-11cf-96B8-444553540000' };
jQuery.nodeName = function( elem, name ) {

		return elem.nodeName && elem.nodeName.toLowerCase() === name.toLowerCase();
	};
jQuery.noop = function() {
/// <summary>
///     An empty function.
/// </summary>
/// <returns type="Function" />
};
jQuery.now = function() {
/// <summary>
///     Return a number representing the current time.
/// </summary>
/// <returns type="Number" />

		return ( new Date() ).getTime();
	};
jQuery.offset = {};
jQuery.param = function( a, traditional ) {
/// <summary>
///     Create a serialized representation of an array or object, suitable for use in a URL query string or Ajax request.
///     &#10;1 - jQuery.param(obj) 
///     &#10;2 - jQuery.param(obj, traditional)
/// </summary>
/// <param name="a" type="Object">
///     An array or object to serialize.
/// </param>
/// <param name="traditional" type="Boolean">
///     A Boolean indicating whether to perform a traditional "shallow" serialization.
/// </param>
/// <returns type="String" />

	var prefix,
		s = [],
		add = function( key, value ) {
			// If value is a function, invoke it and return its value
			value = jQuery.isFunction( value ) ? value() : ( value == null ? "" : value );
			s[ s.length ] = encodeURIComponent( key ) + "=" + encodeURIComponent( value );
		};

	// Set traditional to true for jQuery <= 1.3.2 behavior.
	if ( traditional === undefined ) {
		traditional = jQuery.ajaxSettings && jQuery.ajaxSettings.traditional;
	}

	// If an array was passed in, assume that it is an array of form elements.
	if ( jQuery.isArray( a ) || ( a.jquery && !jQuery.isPlainObject( a ) ) ) {
		// Serialize the form elements
		jQuery.each( a, function() {
			add( this.name, this.value );
		});

	} else {
		// If traditional, encode the "old" way (the way 1.3.2 or older
		// did it), otherwise encode params recursively.
		for ( prefix in a ) {
			buildParams( prefix, a[ prefix ], traditional, add );
		}
	}

	// Return the resulting serialization
	return s.join( "&" ).replace( r20, "+" );
};
jQuery.parseHTML = function( data, context, keepScripts ) {

		if ( !data || typeof data !== "string" ) {
			return null;
		}
		if ( typeof context === "boolean" ) {
			keepScripts = context;
			context = false;
		}
		context = context || document;

		var parsed = rsingleTag.exec( data ),
			scripts = !keepScripts && [];

		// Single tag
		if ( parsed ) {
			return [ context.createElement( parsed[1] ) ];
		}

		parsed = jQuery.buildFragment( [ data ], context, scripts );
		if ( scripts ) {
			jQuery( scripts ).remove();
		}
		return jQuery.merge( [], parsed.childNodes );
	};
jQuery.parseJSON = function( data ) {
/// <summary>
///     Takes a well-formed JSON string and returns the resulting JavaScript object.
/// </summary>
/// <param name="data" type="String">
///     The JSON string to parse.
/// </param>
/// <returns type="Object" />

		// Attempt to parse using the native JSON parser first
		if ( window.JSON && window.JSON.parse ) {
			return window.JSON.parse( data );
		}

		if ( data === null ) {
			return data;
		}

		if ( typeof data === "string" ) {

			// Make sure leading/trailing whitespace is removed (IE can't handle it)
			data = jQuery.trim( data );

			if ( data ) {
				// Make sure the incoming data is actual JSON
				// Logic borrowed from http://json.org/json2.js
				if ( rvalidchars.test( data.replace( rvalidescape, "@" )
					.replace( rvalidtokens, "]" )
					.replace( rvalidbraces, "")) ) {

					return ( new Function( "return " + data ) )();
				}
			}
		}

		jQuery.error( "Invalid JSON: " + data );
	};
jQuery.parseXML = function( data ) {
/// <summary>
///     Parses a string into an XML document.
/// </summary>
/// <param name="data" type="String">
///     a well-formed XML string to be parsed
/// </param>
/// <returns type="XMLDocument" />

		var xml, tmp;
		if ( !data || typeof data !== "string" ) {
			return null;
		}
		try {
			if ( window.DOMParser ) { // Standard
				tmp = new DOMParser();
				xml = tmp.parseFromString( data , "text/xml" );
			} else { // IE
				xml = new ActiveXObject( "Microsoft.XMLDOM" );
				xml.async = "false";
				xml.loadXML( data );
			}
		} catch( e ) {
			xml = undefined;
		}
		if ( !xml || !xml.documentElement || xml.getElementsByTagName( "parsererror" ).length ) {
			jQuery.error( "Invalid XML: " + data );
		}
		return xml;
	};
jQuery.post = function( url, data, callback, type ) {
/// <summary>
///     Load data from the server using a HTTP POST request.
/// </summary>
/// <param name="url" type="String">
///     A string containing the URL to which the request is sent.
/// </param>
/// <param name="data" type="String">
///     A map or string that is sent to the server with the request.
/// </param>
/// <param name="callback" type="Function">
///     A callback function that is executed if the request succeeds.
/// </param>
/// <param name="type" type="String">
///     The type of data expected from the server. Default: Intelligent Guess (xml, json, script, text, html).
/// </param>

		// shift arguments if data argument was omitted
		if ( jQuery.isFunction( data ) ) {
			type = type || callback;
			callback = data;
			data = undefined;
		}

		return jQuery.ajax({
			url: url,
			type: method,
			dataType: type,
			data: data,
			success: callback
		});
	};
jQuery.prop = function( elem, name, value ) {

		var ret, hooks, notxml,
			nType = elem.nodeType;

		// don't get/set properties on text, comment and attribute nodes
		if ( !elem || nType === 3 || nType === 8 || nType === 2 ) {
			return;
		}

		notxml = nType !== 1 || !jQuery.isXMLDoc( elem );

		if ( notxml ) {
			// Fix name and attach hooks
			name = jQuery.propFix[ name ] || name;
			hooks = jQuery.propHooks[ name ];
		}

		if ( value !== undefined ) {
			return hooks && "set" in hooks && (ret = hooks.set( elem, value, name )) !== undefined ?
				ret :
				( elem[ name ] = value );

		} else {
			return hooks && "get" in hooks && (ret = hooks.get( elem, name )) !== null ?
				ret :
				elem[ name ];
		}
	};
jQuery.propFix = { "for": 'htmlFor',
"class": 'className',
"tabindex": {},
"readonly": {},
"maxlength": {},
"cellspacing": {},
"cellpadding": {},
"rowspan": {},
"colspan": {},
"usemap": {},
"frameborder": {},
"contenteditable": {} };
jQuery.propHooks = { "tabIndex": {},
"selected": {} };
jQuery.proxy = function( fn, context ) {
/// <summary>
///     Takes a function and returns a new one that will always have a particular context.
///     &#10;1 - jQuery.proxy(function, context) 
///     &#10;2 - jQuery.proxy(context, name)
/// </summary>
/// <param name="fn" type="Function">
///     The function whose context will be changed.
/// </param>
/// <param name="context" type="Object">
///     The object to which the context (this) of the function should be set.
/// </param>
/// <returns type="Function" />

		var args, proxy, tmp;

		if ( typeof context === "string" ) {
			tmp = fn[ context ];
			context = fn;
			fn = tmp;
		}

		// Quick check to determine if target is callable, in the spec
		// this throws a TypeError, but we will just return undefined.
		if ( !jQuery.isFunction( fn ) ) {
			return undefined;
		}

		// Simulated bind
		args = core_slice.call( arguments, 2 );
		proxy = function() {
			return fn.apply( context || this, args.concat( core_slice.call( arguments ) ) );
		};

		// Set the guid of unique handler to the same of original handler, so it can be removed
		proxy.guid = fn.guid = fn.guid || jQuery.guid++;

		return proxy;
	};
jQuery.queue = function( elem, type, data ) {
/// <summary>
///     1: Show the queue of functions to be executed on the matched element.
///     &#10;    1.1 - jQuery.queue(element, queueName)
///     &#10;2: Manipulate the queue of functions to be executed on the matched element.
///     &#10;    2.1 - jQuery.queue(element, queueName, newQueue) 
///     &#10;    2.2 - jQuery.queue(element, queueName, callback())
/// </summary>
/// <param name="elem" domElement="true">
///     A DOM element where the array of queued functions is attached.
/// </param>
/// <param name="type" type="String">
///     A string containing the name of the queue. Defaults to fx, the standard effects queue.
/// </param>
/// <param name="data" type="Array">
///     An array of functions to replace the current queue contents.
/// </param>
/// <returns type="jQuery" />

		var queue;

		if ( elem ) {
			type = ( type || "fx" ) + "queue";
			queue = jQuery._data( elem, type );

			// Speed up dequeue by getting out quickly if this is just a lookup
			if ( data ) {
				if ( !queue || jQuery.isArray(data) ) {
					queue = jQuery._data( elem, type, jQuery.makeArray(data) );
				} else {
					queue.push( data );
				}
			}
			return queue || [];
		}
	};
jQuery.ready = function( wait ) {


		// Abort if there are pending holds or we're already ready
		if ( wait === true ? --jQuery.readyWait : jQuery.isReady ) {
			return;
		}

		// Make sure body exists, at least, in case IE gets a little overzealous (ticket #5443).
		if ( !document.body ) {
			return setTimeout( jQuery.ready );
		}

		// Remember that the DOM is ready
		jQuery.isReady = true;

		// If a normal DOM Ready event fired, decrement, and wait if need be
		if ( wait !== true && --jQuery.readyWait > 0 ) {
			return;
		}

		// If there are functions bound, to execute
		readyList.resolveWith( document, [ jQuery ] );

		// Trigger any bound ready events
		if ( jQuery.fn.trigger ) {
			jQuery( document ).trigger("ready").off("ready");
		}
	};
jQuery.readyWait = 0;
jQuery.removeAttr = function( elem, value ) {

		var name, propName,
			i = 0,
			attrNames = value && value.match( core_rnotwhite );

		if ( attrNames && elem.nodeType === 1 ) {
			while ( (name = attrNames[i++]) ) {
				propName = jQuery.propFix[ name ] || name;

				// Boolean attributes get special treatment (#10870)
				if ( jQuery.expr.match.bool.test( name ) ) {
					// Set corresponding property to false
					if ( getSetInput && getSetAttribute || !ruseDefault.test( name ) ) {
						elem[ propName ] = false;
					// Support: IE<9
					// Also clear defaultChecked/defaultSelected (if appropriate)
					} else {
						elem[ jQuery.camelCase( "default-" + name ) ] =
							elem[ propName ] = false;
					}

				// See #9699 for explanation of this approach (setting first, then removal)
				} else {
					jQuery.attr( elem, name, "" );
				}

				elem.removeAttribute( getSetAttribute ? name : propName );
			}
		}
	};
jQuery.removeData = function( elem, name ) {
/// <summary>
///     Remove a previously-stored piece of data.
/// </summary>
/// <param name="elem" domElement="true">
///     A DOM element from which to remove data.
/// </param>
/// <param name="name" type="String">
///     A string naming the piece of data to remove.
/// </param>
/// <returns type="jQuery" />

		return internalRemoveData( elem, name );
	};
jQuery.removeEvent = function( elem, type, handle ) {

		if ( elem.removeEventListener ) {
			elem.removeEventListener( type, handle, false );
		}
	};
jQuery.sibling = function( n, elem ) {

		var r = [];

		for ( ; n; n = n.nextSibling ) {
			if ( n.nodeType === 1 && n !== elem ) {
				r.push( n );
			}
		}

		return r;
	};
jQuery.speed = function( speed, easing, fn ) {

	var opt = speed && typeof speed === "object" ? jQuery.extend( {}, speed ) : {
		complete: fn || !fn && easing ||
			jQuery.isFunction( speed ) && speed,
		duration: speed,
		easing: fn && easing || easing && !jQuery.isFunction( easing ) && easing
	};

	opt.duration = jQuery.fx.off ? 0 : typeof opt.duration === "number" ? opt.duration :
		opt.duration in jQuery.fx.speeds ? jQuery.fx.speeds[ opt.duration ] : jQuery.fx.speeds._default;

	// normalize opt.queue - true/undefined/null -> "fx"
	if ( opt.queue == null || opt.queue === true ) {
		opt.queue = "fx";
	}

	// Queueing
	opt.old = opt.complete;

	opt.complete = function() {
		if ( jQuery.isFunction( opt.old ) ) {
			opt.old.call( this );
		}

		if ( opt.queue ) {
			jQuery.dequeue( this, opt.queue );
		}
	};

	return opt;
};
jQuery.style = function( elem, name, value, extra ) {

		// Don't set styles on text and comment nodes
		if ( !elem || elem.nodeType === 3 || elem.nodeType === 8 || !elem.style ) {
			return;
		}

		// Make sure that we're working with the right name
		var ret, type, hooks,
			origName = jQuery.camelCase( name ),
			style = elem.style;

		name = jQuery.cssProps[ origName ] || ( jQuery.cssProps[ origName ] = vendorPropName( style, origName ) );

		// gets hook for the prefixed version
		// followed by the unprefixed version
		hooks = jQuery.cssHooks[ name ] || jQuery.cssHooks[ origName ];

		// Check if we're setting a value
		if ( value !== undefined ) {
			type = typeof value;

			// convert relative number strings (+= or -=) to relative numbers. #7345
			if ( type === "string" && (ret = rrelNum.exec( value )) ) {
				value = ( ret[1] + 1 ) * ret[2] + parseFloat( jQuery.css( elem, name ) );
				// Fixes bug #9237
				type = "number";
			}

			// Make sure that NaN and null values aren't set. See: #7116
			if ( value == null || type === "number" && isNaN( value ) ) {
				return;
			}

			// If a number was passed in, add 'px' to the (except for certain CSS properties)
			if ( type === "number" && !jQuery.cssNumber[ origName ] ) {
				value += "px";
			}

			// Fixes #8908, it can be done more correctly by specifing setters in cssHooks,
			// but it would mean to define eight (for every problematic property) identical functions
			if ( !jQuery.support.clearCloneStyle && value === "" && name.indexOf("background") === 0 ) {
				style[ name ] = "inherit";
			}

			// If a hook was provided, use that value, otherwise just set the specified value
			if ( !hooks || !("set" in hooks) || (value = hooks.set( elem, value, extra )) !== undefined ) {

				// Wrapped to prevent IE from throwing errors when 'invalid' values are provided
				// Fixes bug #5509
				try {
					style[ name ] = value;
				} catch(e) {}
			}

		} else {
			// If a hook was provided get the non-computed value from there
			if ( hooks && "get" in hooks && (ret = hooks.get( elem, false, extra )) !== undefined ) {
				return ret;
			}

			// Otherwise just get the value from the style object
			return style[ name ];
		}
	};
jQuery.support = { "getSetAttribute": true,
"leadingWhitespace": true,
"tbody": true,
"htmlSerialize": true,
"style": true,
"hrefNormalized": true,
"opacity": true,
"cssFloat": true,
"checkOn": true,
"optSelected": false,
"enctype": true,
"html5Clone": true,
"inlineBlockNeedsLayout": false,
"shrinkWrapBlocks": false,
"pixelPosition": true,
"deleteExpando": true,
"noCloneEvent": true,
"reliableMarginRight": true,
"boxSizingReliable": false,
"noCloneChecked": false,
"optDisabled": true,
"input": true,
"radioValue": false,
"appendChecked": true,
"checkClone": true,
"submitBubbles": true,
"changeBubbles": true,
"focusinBubbles": true,
"clearCloneStyle": false,
"ownLast": false,
"cors": true,
"ajax": true,
"reliableHiddenOffsets": true,
"boxSizing": true };
jQuery.swap = function( elem, options, callback, args ) {

		var ret, name,
			old = {};

		// Remember the old values, and insert the new ones
		for ( name in options ) {
			old[ name ] = elem.style[ name ];
			elem.style[ name ] = options[ name ];
		}

		ret = callback.apply( elem, args || [] );

		// Revert the old values
		for ( name in options ) {
			elem.style[ name ] = old[ name ];
		}

		return ret;
	};
jQuery.text = function( elem ) {

	var node,
		ret = "",
		i = 0,
		nodeType = elem.nodeType;

	if ( !nodeType ) {
		// If no nodeType, this is expected to be an array
		for ( ; (node = elem[i]); i++ ) {
			// Do not traverse comment nodes
			ret += getText( node );
		}
	} else if ( nodeType === 1 || nodeType === 9 || nodeType === 11 ) {
		// Use textContent for elements
		// innerText usage removed for consistency of new lines (see #11153)
		if ( typeof elem.textContent === "string" ) {
			return elem.textContent;
		} else {
			// Traverse its children
			for ( elem = elem.firstChild; elem; elem = elem.nextSibling ) {
				ret += getText( elem );
			}
		}
	} else if ( nodeType === 3 || nodeType === 4 ) {
		return elem.nodeValue;
	}
	// Do not include comment or processing instruction nodes

	return ret;
};
jQuery.trim = function( text ) {
/// <summary>
///     Remove the whitespace from the beginning and end of a string.
/// </summary>
/// <param name="text" type="String">
///     The string to trim.
/// </param>
/// <returns type="String" />

			return text == null ?
				"" :
				core_trim.call( text );
		};
jQuery.type = function( obj ) {
/// <summary>
///     Determine the internal JavaScript [[Class]] of an object.
/// </summary>
/// <param name="obj" type="Object">
///     Object to get the internal JavaScript [[Class]] of.
/// </param>
/// <returns type="String" />

		if ( obj == null ) {
			return String( obj );
		}
		return typeof obj === "object" || typeof obj === "function" ?
			class2type[ core_toString.call(obj) ] || "object" :
			typeof obj;
	};
jQuery.unique = function( results ) {
/// <summary>
///     Sorts an array of DOM elements, in place, with the duplicates removed. Note that this only works on arrays of DOM elements, not strings or numbers.
/// </summary>
/// <param name="results" type="Array">
///     The Array of DOM elements.
/// </param>
/// <returns type="Array" />

	var elem,
		duplicates = [],
		j = 0,
		i = 0;

	// Unless we *know* we can detect duplicates, assume their presence
	hasDuplicate = !support.detectDuplicates;
	sortInput = !support.sortStable && results.slice( 0 );
	results.sort( sortOrder );

	if ( hasDuplicate ) {
		while ( (elem = results[i++]) ) {
			if ( elem === results[ i ] ) {
				j = duplicates.push( i );
			}
		}
		while ( j-- ) {
			results.splice( duplicates[ j ], 1 );
		}
	}

	return results;
};
jQuery.valHooks = { "option": {},
"select": {},
"radio": {},
"checkbox": {} };
jQuery.when = function( subordinate /* , ..., subordinateN */ ) {
/// <summary>
///     Provides a way to execute callback functions based on one or more objects, usually Deferred objects that represent asynchronous events.
/// </summary>
/// <param name="subordinate/*" type="Deferred">
///     One or more Deferred objects, or plain JavaScript objects.
/// </param>
/// <returns type="Promise" />

		var i = 0,
			resolveValues = core_slice.call( arguments ),
			length = resolveValues.length,

			// the count of uncompleted subordinates
			remaining = length !== 1 || ( subordinate && jQuery.isFunction( subordinate.promise ) ) ? length : 0,

			// the master Deferred. If resolveValues consist of only a single Deferred, just use that.
			deferred = remaining === 1 ? subordinate : jQuery.Deferred(),

			// Update function for both resolve and progress values
			updateFunc = function( i, contexts, values ) {
				return function( value ) {
					contexts[ i ] = this;
					values[ i ] = arguments.length > 1 ? core_slice.call( arguments ) : value;
					if( values === progressValues ) {
						deferred.notifyWith( contexts, values );
					} else if ( !( --remaining ) ) {
						deferred.resolveWith( contexts, values );
					}
				};
			},

			progressValues, progressContexts, resolveContexts;

		// add listeners to Deferred subordinates; treat others as resolved
		if ( length > 1 ) {
			progressValues = new Array( length );
			progressContexts = new Array( length );
			resolveContexts = new Array( length );
			for ( ; i < length; i++ ) {
				if ( resolveValues[ i ] && jQuery.isFunction( resolveValues[ i ].promise ) ) {
					resolveValues[ i ].promise()
						.done( updateFunc( i, resolveContexts, resolveValues ) )
						.fail( deferred.reject )
						.progress( updateFunc( i, progressContexts, progressValues ) );
				} else {
					--remaining;
				}
			}
		}

		// if we're not waiting on anything, resolve the master
		if ( !remaining ) {
			deferred.resolveWith( resolveContexts, resolveValues );
		}

		return deferred.promise();
	};
jQuery.Event.prototype.isDefaultPrevented = function returnFalse() {
/// <summary>
///     Returns whether event.preventDefault() was ever called on this event object.
/// </summary>
/// <returns type="Boolean" />

	return false;
};
jQuery.Event.prototype.isImmediatePropagationStopped = function returnFalse() {
/// <summary>
///     Returns whether event.stopImmediatePropagation() was ever called on this event object.
/// </summary>
/// <returns type="Boolean" />

	return false;
};
jQuery.Event.prototype.isPropagationStopped = function returnFalse() {
/// <summary>
///     Returns whether event.stopPropagation() was ever called on this event object.
/// </summary>
/// <returns type="Boolean" />

	return false;
};
jQuery.Event.prototype.preventDefault = function() {
/// <summary>
///     If this method is called, the default action of the event will not be triggered.
/// </summary>
/// <returns type="undefined" />

		var e = this.originalEvent;

		this.isDefaultPrevented = returnTrue;
		if ( !e ) {
			return;
		}

		// If preventDefault exists, run it on the original event
		if ( e.preventDefault ) {
			e.preventDefault();

		// Support: IE
		// Otherwise set the returnValue property of the original event to false
		} else {
			e.returnValue = false;
		}
	};
jQuery.Event.prototype.stopImmediatePropagation = function() {
/// <summary>
///     Keeps the rest of the handlers from being executed and prevents the event from bubbling up the DOM tree.
/// </summary>

		this.isImmediatePropagationStopped = returnTrue;
		this.stopPropagation();
	};
jQuery.Event.prototype.stopPropagation = function() {
/// <summary>
///     Prevents the event from bubbling up the DOM tree, preventing any parent handlers from being notified of the event.
/// </summary>

		var e = this.originalEvent;

		this.isPropagationStopped = returnTrue;
		if ( !e ) {
			return;
		}
		// If stopPropagation exists, run it on the original event
		if ( e.stopPropagation ) {
			e.stopPropagation();
		}

		// Support: IE
		// Set the cancelBubble property of the original event to true
		e.cancelBubble = true;
	};
jQuery.prototype.add = function( selector, context ) {
/// <summary>
///     Add elements to the set of matched elements.
///     &#10;1 - add(selector) 
///     &#10;2 - add(elements) 
///     &#10;3 - add(html) 
///     &#10;4 - add(jQuery object) 
///     &#10;5 - add(selector, context)
/// </summary>
/// <param name="selector" type="String">
///     A string representing a selector expression to find additional elements to add to the set of matched elements.
/// </param>
/// <param name="context" domElement="true">
///     The point in the document at which the selector should begin matching; similar to the context argument of the $(selector, context) method.
/// </param>
/// <returns type="jQuery" />

		var set = typeof selector === "string" ?
				jQuery( selector, context ) :
				jQuery.makeArray( selector && selector.nodeType ? [ selector ] : selector ),
			all = jQuery.merge( this.get(), set );

		return this.pushStack( jQuery.unique(all) );
	};
jQuery.prototype.addBack = function( selector ) {

		return this.add( selector == null ?
			this.prevObject : this.prevObject.filter(selector)
		);
	};
jQuery.prototype.addClass = function( value ) {
/// <summary>
///     Adds the specified class(es) to each of the set of matched elements.
///     &#10;1 - addClass(className) 
///     &#10;2 - addClass(function(index, currentClass))
/// </summary>
/// <param name="value" type="String">
///     One or more class names to be added to the class attribute of each matched element.
/// </param>
/// <returns type="jQuery" />

		var classes, elem, cur, clazz, j,
			i = 0,
			len = this.length,
			proceed = typeof value === "string" && value;

		if ( jQuery.isFunction( value ) ) {
			return this.each(function( j ) {
				jQuery( this ).addClass( value.call( this, j, this.className ) );
			});
		}

		if ( proceed ) {
			// The disjunction here is for better compressibility (see removeClass)
			classes = ( value || "" ).match( core_rnotwhite ) || [];

			for ( ; i < len; i++ ) {
				elem = this[ i ];
				cur = elem.nodeType === 1 && ( elem.className ?
					( " " + elem.className + " " ).replace( rclass, " " ) :
					" "
				);

				if ( cur ) {
					j = 0;
					while ( (clazz = classes[j++]) ) {
						if ( cur.indexOf( " " + clazz + " " ) < 0 ) {
							cur += clazz + " ";
						}
					}
					elem.className = jQuery.trim( cur );

				}
			}
		}

		return this;
	};
jQuery.prototype.after = function() {
/// <summary>
///     Insert content, specified by the parameter, after each element in the set of matched elements.
///     &#10;1 - after(content, content) 
///     &#10;2 - after(function(index))
/// </summary>
/// <param name="" type="jQuery">
///     HTML string, DOM element, or jQuery object to insert after each element in the set of matched elements.
/// </param>
/// <param name="" type="jQuery">
///     One or more additional DOM elements, arrays of elements, HTML strings, or jQuery objects to insert after each element in the set of matched elements.
/// </param>
/// <returns type="jQuery" />

		return this.domManip( arguments, function( elem ) {
			if ( this.parentNode ) {
				this.parentNode.insertBefore( elem, this.nextSibling );
			}
		});
	};
jQuery.prototype.ajaxComplete = function( fn ){
/// <summary>
///     Register a handler to be called when Ajax requests complete. This is an Ajax Event.
/// </summary>
/// <param name="fn" type="Function">
///     The function to be invoked.
/// </param>
/// <returns type="jQuery" />

		return this.on( type, fn );
	};
jQuery.prototype.ajaxError = function( fn ){
/// <summary>
///     Register a handler to be called when Ajax requests complete with an error. This is an Ajax Event.
/// </summary>
/// <param name="fn" type="Function">
///     The function to be invoked.
/// </param>
/// <returns type="jQuery" />

		return this.on( type, fn );
	};
jQuery.prototype.ajaxSend = function( fn ){
/// <summary>
///     Attach a function to be executed before an Ajax request is sent. This is an Ajax Event.
/// </summary>
/// <param name="fn" type="Function">
///     The function to be invoked.
/// </param>
/// <returns type="jQuery" />

		return this.on( type, fn );
	};
jQuery.prototype.ajaxStart = function( fn ){
/// <summary>
///     Register a handler to be called when the first Ajax request begins. This is an Ajax Event.
/// </summary>
/// <param name="fn" type="Function">
///     The function to be invoked.
/// </param>
/// <returns type="jQuery" />

		return this.on( type, fn );
	};
jQuery.prototype.ajaxStop = function( fn ){
/// <summary>
///     Register a handler to be called when all Ajax requests have completed. This is an Ajax Event.
/// </summary>
/// <param name="fn" type="Function">
///     The function to be invoked.
/// </param>
/// <returns type="jQuery" />

		return this.on( type, fn );
	};
jQuery.prototype.ajaxSuccess = function( fn ){
/// <summary>
///     Attach a function to be executed whenever an Ajax request completes successfully. This is an Ajax Event.
/// </summary>
/// <param name="fn" type="Function">
///     The function to be invoked.
/// </param>
/// <returns type="jQuery" />

		return this.on( type, fn );
	};
jQuery.prototype.andSelf = function( selector ) {
/// <summary>
///     Add the previous set of elements on the stack to the current set.
/// </summary>
/// <returns type="jQuery" />

		return this.add( selector == null ?
			this.prevObject : this.prevObject.filter(selector)
		);
	};
jQuery.prototype.animate = function( prop, speed, easing, callback ) {
/// <summary>
///     Perform a custom animation of a set of CSS properties.
///     &#10;1 - animate(properties, duration, easing, complete) 
///     &#10;2 - animate(properties, options)
/// </summary>
/// <param name="prop" type="Object">
///     A map of CSS properties that the animation will move toward.
/// </param>
/// <param name="speed" type="Number">
///     A string or number determining how long the animation will run.
/// </param>
/// <param name="easing" type="String">
///     A string indicating which easing function to use for the transition.
/// </param>
/// <param name="callback" type="Function">
///     A function to call once the animation is complete.
/// </param>
/// <returns type="jQuery" />

		var empty = jQuery.isEmptyObject( prop ),
			optall = jQuery.speed( speed, easing, callback ),
			doAnimation = function() {
				// Operate on a copy of prop so per-property easing won't be lost
				var anim = Animation( this, jQuery.extend( {}, prop ), optall );
				doAnimation.finish = function() {
					anim.stop( true );
				};
				// Empty animations, or finishing resolves immediately
				if ( empty || jQuery._data( this, "finish" ) ) {
					anim.stop( true );
				}
			};
			doAnimation.finish = doAnimation;

		return empty || optall.queue === false ?
			this.each( doAnimation ) :
			this.queue( optall.queue, doAnimation );
	};
jQuery.prototype.append = function() {
/// <summary>
///     Insert content, specified by the parameter, to the end of each element in the set of matched elements.
///     &#10;1 - append(content, content) 
///     &#10;2 - append(function(index, html))
/// </summary>
/// <param name="" type="jQuery">
///     DOM element, HTML string, or jQuery object to insert at the end of each element in the set of matched elements.
/// </param>
/// <param name="" type="jQuery">
///     One or more additional DOM elements, arrays of elements, HTML strings, or jQuery objects to insert at the end of each element in the set of matched elements.
/// </param>
/// <returns type="jQuery" />

		return this.domManip( arguments, function( elem ) {
			if ( this.nodeType === 1 || this.nodeType === 11 || this.nodeType === 9 ) {
				var target = manipulationTarget( this, elem );
				target.appendChild( elem );
			}
		});
	};
jQuery.prototype.appendTo = function( selector ) {
/// <summary>
///     Insert every element in the set of matched elements to the end of the target.
/// </summary>
/// <param name="selector" type="jQuery">
///     A selector, element, HTML string, or jQuery object; the matched set of elements will be inserted at the end of the element(s) specified by this parameter.
/// </param>
/// <returns type="jQuery" />

		var elems,
			i = 0,
			ret = [],
			insert = jQuery( selector ),
			last = insert.length - 1;

		for ( ; i <= last; i++ ) {
			elems = i === last ? this : this.clone(true);
			jQuery( insert[i] )[ original ]( elems );

			// Modern browsers can apply jQuery collections as arrays, but oldIE needs a .get()
			core_push.apply( ret, elems.get() );
		}

		return this.pushStack( ret );
	};
jQuery.prototype.attr = function( name, value ) {
/// <summary>
///     1: Get the value of an attribute for the first element in the set of matched elements.
///     &#10;    1.1 - attr(attributeName)
///     &#10;2: Set one or more attributes for the set of matched elements.
///     &#10;    2.1 - attr(attributeName, value) 
///     &#10;    2.2 - attr(map) 
///     &#10;    2.3 - attr(attributeName, function(index, attr))
/// </summary>
/// <param name="name" type="String">
///     The name of the attribute to set.
/// </param>
/// <param name="value" type="Number">
///     A value to set for the attribute.
/// </param>
/// <returns type="jQuery" />

		return jQuery.access( this, jQuery.attr, name, value, arguments.length > 1 );
	};
jQuery.prototype.before = function() {
/// <summary>
///     Insert content, specified by the parameter, before each element in the set of matched elements.
///     &#10;1 - before(content, content) 
///     &#10;2 - before(function)
/// </summary>
/// <param name="" type="jQuery">
///     HTML string, DOM element, or jQuery object to insert before each element in the set of matched elements.
/// </param>
/// <param name="" type="jQuery">
///     One or more additional DOM elements, arrays of elements, HTML strings, or jQuery objects to insert before each element in the set of matched elements.
/// </param>
/// <returns type="jQuery" />

		return this.domManip( arguments, function( elem ) {
			if ( this.parentNode ) {
				this.parentNode.insertBefore( elem, this );
			}
		});
	};
jQuery.prototype.bind = function( types, data, fn ) {
/// <summary>
///     Attach a handler to an event for the elements.
///     &#10;1 - bind(eventType, eventData, handler(eventObject)) 
///     &#10;2 - bind(eventType, eventData, preventBubble) 
///     &#10;3 - bind(events)
/// </summary>
/// <param name="types" type="String">
///     A string containing one or more DOM event types, such as "click" or "submit," or custom event names.
/// </param>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return this.on( types, null, data, fn );
	};
jQuery.prototype.blur = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "blur" JavaScript event, or trigger that event on an element.
///     &#10;1 - blur(handler(eventObject)) 
///     &#10;2 - blur(eventData, handler(eventObject)) 
///     &#10;3 - blur()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.change = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "change" JavaScript event, or trigger that event on an element.
///     &#10;1 - change(handler(eventObject)) 
///     &#10;2 - change(eventData, handler(eventObject)) 
///     &#10;3 - change()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.children = function( until, selector ) {
/// <summary>
///     Get the children of each element in the set of matched elements, optionally filtered by a selector.
/// </summary>
/// <param name="until" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="jQuery" />

		var ret = jQuery.map( this, fn, until );

		if ( name.slice( -5 ) !== "Until" ) {
			selector = until;
		}

		if ( selector && typeof selector === "string" ) {
			ret = jQuery.filter( selector, ret );
		}

		if ( this.length > 1 ) {
			// Remove duplicates
			if ( !guaranteedUnique[ name ] ) {
				ret = jQuery.unique( ret );
			}

			// Reverse order for parents* and prev-derivatives
			if ( rparentsprev.test( name ) ) {
				ret = ret.reverse();
			}
		}

		return this.pushStack( ret );
	};
jQuery.prototype.clearQueue = function( type ) {
/// <summary>
///     Remove from the queue all items that have not yet been run.
/// </summary>
/// <param name="type" type="String">
///     A string containing the name of the queue. Defaults to fx, the standard effects queue.
/// </param>
/// <returns type="jQuery" />

		return this.queue( type || "fx", [] );
	};
jQuery.prototype.click = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "click" JavaScript event, or trigger that event on an element.
///     &#10;1 - click(handler(eventObject)) 
///     &#10;2 - click(eventData, handler(eventObject)) 
///     &#10;3 - click()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.clone = function( dataAndEvents, deepDataAndEvents ) {
/// <summary>
///     Create a deep copy of the set of matched elements.
///     &#10;1 - clone(withDataAndEvents) 
///     &#10;2 - clone(withDataAndEvents, deepWithDataAndEvents)
/// </summary>
/// <param name="dataAndEvents" type="Boolean">
///     A Boolean indicating whether event handlers and data should be copied along with the elements. The default value is false. *In jQuery 1.5.0 the default value was incorrectly true; it was changed back to false in 1.5.1 and up.
/// </param>
/// <param name="deepDataAndEvents" type="Boolean">
///     A Boolean indicating whether event handlers and data for all children of the cloned element should be copied. By default its value matches the first argument's value (which defaults to false).
/// </param>
/// <returns type="jQuery" />

		dataAndEvents = dataAndEvents == null ? false : dataAndEvents;
		deepDataAndEvents = deepDataAndEvents == null ? dataAndEvents : deepDataAndEvents;

		return this.map( function () {
			return jQuery.clone( this, dataAndEvents, deepDataAndEvents );
		});
	};
jQuery.prototype.closest = function( selectors, context ) {
/// <summary>
///     1: Get the first element that matches the selector, beginning at the current element and progressing up through the DOM tree.
///     &#10;    1.1 - closest(selector) 
///     &#10;    1.2 - closest(selector, context) 
///     &#10;    1.3 - closest(jQuery object) 
///     &#10;    1.4 - closest(element)
///     &#10;2: Gets an array of all the elements and selectors matched against the current element up through the DOM tree.
///     &#10;    2.1 - closest(selectors, context)
/// </summary>
/// <param name="selectors" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <param name="context" domElement="true">
///     A DOM element within which a matching element may be found. If no context is passed in then the context of the jQuery set will be used instead.
/// </param>
/// <returns type="jQuery" />

		var cur,
			i = 0,
			l = this.length,
			ret = [],
			pos = rneedsContext.test( selectors ) || typeof selectors !== "string" ?
				jQuery( selectors, context || this.context ) :
				0;

		for ( ; i < l; i++ ) {
			for ( cur = this[i]; cur && cur !== context; cur = cur.parentNode ) {
				// Always skip document fragments
				if ( cur.nodeType < 11 && (pos ?
					pos.index(cur) > -1 :

					// Don't pass non-elements to Sizzle
					cur.nodeType === 1 &&
						jQuery.find.matchesSelector(cur, selectors)) ) {

					cur = ret.push( cur );
					break;
				}
			}
		}

		return this.pushStack( ret.length > 1 ? jQuery.unique( ret ) : ret );
	};
jQuery.prototype.constructor = function( selector, context ) {

		// The jQuery object is actually just the init constructor 'enhanced'
		return new jQuery.fn.init( selector, context, rootjQuery );
	};
jQuery.prototype.contents = function( until, selector ) {
/// <summary>
///     Get the children of each element in the set of matched elements, including text and comment nodes.
/// </summary>
/// <returns type="jQuery" />

		var ret = jQuery.map( this, fn, until );

		if ( name.slice( -5 ) !== "Until" ) {
			selector = until;
		}

		if ( selector && typeof selector === "string" ) {
			ret = jQuery.filter( selector, ret );
		}

		if ( this.length > 1 ) {
			// Remove duplicates
			if ( !guaranteedUnique[ name ] ) {
				ret = jQuery.unique( ret );
			}

			// Reverse order for parents* and prev-derivatives
			if ( rparentsprev.test( name ) ) {
				ret = ret.reverse();
			}
		}

		return this.pushStack( ret );
	};
jQuery.prototype.contextmenu = function( data, fn ) {

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.css = function( name, value ) {
/// <summary>
///     1: Get the value of a style property for the first element in the set of matched elements.
///     &#10;    1.1 - css(propertyName)
///     &#10;2: Set one or more CSS properties for the  set of matched elements.
///     &#10;    2.1 - css(propertyName, value) 
///     &#10;    2.2 - css(propertyName, function(index, value)) 
///     &#10;    2.3 - css(map)
/// </summary>
/// <param name="name" type="String">
///     A CSS property name.
/// </param>
/// <param name="value" type="Number">
///     A value to set for the property.
/// </param>
/// <returns type="jQuery" />

		return jQuery.access( this, function( elem, name, value ) {
			var len, styles,
				map = {},
				i = 0;

			if ( jQuery.isArray( name ) ) {
				styles = getStyles( elem );
				len = name.length;

				for ( ; i < len; i++ ) {
					map[ name[ i ] ] = jQuery.css( elem, name[ i ], false, styles );
				}

				return map;
			}

			return value !== undefined ?
				jQuery.style( elem, name, value ) :
				jQuery.css( elem, name );
		}, name, value, arguments.length > 1 );
	};
jQuery.prototype.data = function( key, value ) {
/// <summary>
///     1: Store arbitrary data associated with the matched elements.
///     &#10;    1.1 - data(key, value) 
///     &#10;    1.2 - data(obj)
///     &#10;2: Returns value at named data store for the first element in the jQuery collection, as set by data(name, value).
///     &#10;    2.1 - data(key) 
///     &#10;    2.2 - data()
/// </summary>
/// <param name="key" type="String">
///     A string naming the piece of data to set.
/// </param>
/// <param name="value" type="Object">
///     The new data value; it can be any Javascript type including Array or Object.
/// </param>
/// <returns type="jQuery" />

		var attrs, name,
			data = null,
			i = 0,
			elem = this[0];

		// Special expections of .data basically thwart jQuery.access,
		// so implement the relevant behavior ourselves

		// Gets all values
		if ( key === undefined ) {
			if ( this.length ) {
				data = jQuery.data( elem );

				if ( elem.nodeType === 1 && !jQuery._data( elem, "parsedAttrs" ) ) {
					attrs = elem.attributes;
					for ( ; i < attrs.length; i++ ) {
						name = attrs[i].name;

						if ( name.indexOf("data-") === 0 ) {
							name = jQuery.camelCase( name.slice(5) );

							dataAttr( elem, name, data[ name ] );
						}
					}
					jQuery._data( elem, "parsedAttrs", true );
				}
			}

			return data;
		}

		// Sets multiple values
		if ( typeof key === "object" ) {
			return this.each(function() {
				jQuery.data( this, key );
			});
		}

		return arguments.length > 1 ?

			// Sets one value
			this.each(function() {
				jQuery.data( this, key, value );
			}) :

			// Gets one value
			// Try to fetch any internally stored data first
			elem ? dataAttr( elem, key, jQuery.data( elem, key ) ) : null;
	};
jQuery.prototype.dblclick = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "dblclick" JavaScript event, or trigger that event on an element.
///     &#10;1 - dblclick(handler(eventObject)) 
///     &#10;2 - dblclick(eventData, handler(eventObject)) 
///     &#10;3 - dblclick()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.delay = function( time, type ) {
/// <summary>
///     Set a timer to delay execution of subsequent items in the queue.
/// </summary>
/// <param name="time" type="Number">
///     An integer indicating the number of milliseconds to delay execution of the next item in the queue.
/// </param>
/// <param name="type" type="String">
///     A string containing the name of the queue. Defaults to fx, the standard effects queue.
/// </param>
/// <returns type="jQuery" />

		time = jQuery.fx ? jQuery.fx.speeds[ time ] || time : time;
		type = type || "fx";

		return this.queue( type, function( next, hooks ) {
			var timeout = setTimeout( next, time );
			hooks.stop = function() {
				clearTimeout( timeout );
			};
		});
	};
jQuery.prototype.delegate = function( selector, types, data, fn ) {
/// <summary>
///     Attach a handler to one or more events for all elements that match the selector, now or in the future, based on a specific set of root elements.
///     &#10;1 - delegate(selector, eventType, handler(eventObject)) 
///     &#10;2 - delegate(selector, eventType, eventData, handler(eventObject)) 
///     &#10;3 - delegate(selector, events)
/// </summary>
/// <param name="selector" type="String">
///     A selector to filter the elements that trigger the event.
/// </param>
/// <param name="types" type="String">
///     A string containing one or more space-separated JavaScript event types, such as "click" or "keydown," or custom event names.
/// </param>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute at the time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return this.on( types, selector, data, fn );
	};
jQuery.prototype.dequeue = function( type ) {
/// <summary>
///     Execute the next function on the queue for the matched elements.
/// </summary>
/// <param name="type" type="String">
///     A string containing the name of the queue. Defaults to fx, the standard effects queue.
/// </param>
/// <returns type="jQuery" />

		return this.each(function() {
			jQuery.dequeue( this, type );
		});
	};
jQuery.prototype.detach = function( selector ) {
/// <summary>
///     Remove the set of matched elements from the DOM.
/// </summary>
/// <param name="selector" type="String">
///     A selector expression that filters the set of matched elements to be removed.
/// </param>
/// <returns type="jQuery" />

		return this.remove( selector, true );
	};
jQuery.prototype.domManip = function( args, callback, allowIntersection ) {


		// Flatten any nested arrays
		args = core_concat.apply( [], args );

		var first, node, hasScripts,
			scripts, doc, fragment,
			i = 0,
			l = this.length,
			set = this,
			iNoClone = l - 1,
			value = args[0],
			isFunction = jQuery.isFunction( value );

		// We can't cloneNode fragments that contain checked, in WebKit
		if ( isFunction || !( l <= 1 || typeof value !== "string" || jQuery.support.checkClone || !rchecked.test( value ) ) ) {
			return this.each(function( index ) {
				var self = set.eq( index );
				if ( isFunction ) {
					args[0] = value.call( this, index, self.html() );
				}
				self.domManip( args, callback, allowIntersection );
			});
		}

		if ( l ) {
			fragment = jQuery.buildFragment( args, this[ 0 ].ownerDocument, false, !allowIntersection && this );
			first = fragment.firstChild;

			if ( fragment.childNodes.length === 1 ) {
				fragment = first;
			}

			if ( first ) {
				scripts = jQuery.map( getAll( fragment, "script" ), disableScript );
				hasScripts = scripts.length;

				// Use the original fragment for the last item instead of the first because it can end up
				// being emptied incorrectly in certain situations (#8070).
				for ( ; i < l; i++ ) {
					node = fragment;

					if ( i !== iNoClone ) {
						node = jQuery.clone( node, true, true );

						// Keep references to cloned scripts for later restoration
						if ( hasScripts ) {
							jQuery.merge( scripts, getAll( node, "script" ) );
						}
					}

					callback.call( this[i], node, i );
				}

				if ( hasScripts ) {
					doc = scripts[ scripts.length - 1 ].ownerDocument;

					// Reenable scripts
					jQuery.map( scripts, restoreScript );

					// Evaluate executable scripts on first document insertion
					for ( i = 0; i < hasScripts; i++ ) {
						node = scripts[ i ];
						if ( rscriptType.test( node.type || "" ) &&
							!jQuery._data( node, "globalEval" ) && jQuery.contains( doc, node ) ) {

							if ( node.src ) {
								// Hope ajax is available...
								jQuery._evalUrl( node.src );
							} else {
								jQuery.globalEval( ( node.text || node.textContent || node.innerHTML || "" ).replace( rcleanScript, "" ) );
							}
						}
					}
				}

				// Fix #11809: Avoid leaking memory
				fragment = first = null;
			}
		}

		return this;
	};
jQuery.prototype.each = function( callback, args ) {
/// <summary>
///     Iterate over a jQuery object, executing a function for each matched element.
/// </summary>
/// <param name="callback" type="Function">
///     A function to execute for each matched element.
/// </param>
/// <returns type="jQuery" />

		return jQuery.each( this, callback, args );
	};
jQuery.prototype.empty = function() {
/// <summary>
///     Remove all child nodes of the set of matched elements from the DOM.
/// </summary>
/// <returns type="jQuery" />

		var elem,
			i = 0;

		for ( ; (elem = this[i]) != null; i++ ) {
			// Remove element nodes and prevent memory leaks
			if ( elem.nodeType === 1 ) {
				jQuery.cleanData( getAll( elem, false ) );
			}

			// Remove any remaining nodes
			while ( elem.firstChild ) {
				elem.removeChild( elem.firstChild );
			}

			// If this is a select, ensure that it displays empty (#12336)
			// Support: IE<9
			if ( elem.options && jQuery.nodeName( elem, "select" ) ) {
				elem.options.length = 0;
			}
		}

		return this;
	};
jQuery.prototype.end = function() {
/// <summary>
///     End the most recent filtering operation in the current chain and return the set of matched elements to its previous state.
/// </summary>
/// <returns type="jQuery" />

		return this.prevObject || this.constructor(null);
	};
jQuery.prototype.eq = function( i ) {
/// <summary>
///     Reduce the set of matched elements to the one at the specified index.
///     &#10;1 - eq(index) 
///     &#10;2 - eq(-index)
/// </summary>
/// <param name="i" type="Number">
///     An integer indicating the 0-based position of the element.
/// </param>
/// <returns type="jQuery" />

		var len = this.length,
			j = +i + ( i < 0 ? len : 0 );
		return this.pushStack( j >= 0 && j < len ? [ this[j] ] : [] );
	};
jQuery.prototype.error = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "error" JavaScript event.
///     &#10;1 - error(handler(eventObject)) 
///     &#10;2 - error(eventData, handler(eventObject))
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.extend = function() {

	var src, copyIsArray, copy, name, options, clone,
		target = arguments[0] || {},
		i = 1,
		length = arguments.length,
		deep = false;

	// Handle a deep copy situation
	if ( typeof target === "boolean" ) {
		deep = target;
		target = arguments[1] || {};
		// skip the boolean and the target
		i = 2;
	}

	// Handle case when target is a string or something (possible in deep copy)
	if ( typeof target !== "object" && !jQuery.isFunction(target) ) {
		target = {};
	}

	// extend jQuery itself if only one argument is passed
	if ( length === i ) {
		target = this;
		--i;
	}

	for ( ; i < length; i++ ) {
		// Only deal with non-null/undefined values
		if ( (options = arguments[ i ]) != null ) {
			// Extend the base object
			for ( name in options ) {
				src = target[ name ];
				copy = options[ name ];

				// Prevent never-ending loop
				if ( target === copy ) {
					continue;
				}

				// Recurse if we're merging plain objects or arrays
				if ( deep && copy && ( jQuery.isPlainObject(copy) || (copyIsArray = jQuery.isArray(copy)) ) ) {
					if ( copyIsArray ) {
						copyIsArray = false;
						clone = src && jQuery.isArray(src) ? src : [];

					} else {
						clone = src && jQuery.isPlainObject(src) ? src : {};
					}

					// Never move original objects, clone them
					target[ name ] = jQuery.extend( deep, clone, copy );

				// Don't bring in undefined values
				} else if ( copy !== undefined ) {
					target[ name ] = copy;
				}
			}
		}
	}

	// Return the modified object
	return target;
};
jQuery.prototype.fadeIn = function( speed, easing, callback ) {
/// <summary>
///     Display the matched elements by fading them to opaque.
///     &#10;1 - fadeIn(duration, callback) 
///     &#10;2 - fadeIn(duration, easing, callback)
/// </summary>
/// <param name="speed" type="Number">
///     A string or number determining how long the animation will run.
/// </param>
/// <param name="easing" type="String">
///     A string indicating which easing function to use for the transition.
/// </param>
/// <param name="callback" type="Function">
///     A function to call once the animation is complete.
/// </param>
/// <returns type="jQuery" />

		return this.animate( props, speed, easing, callback );
	};
jQuery.prototype.fadeOut = function( speed, easing, callback ) {
/// <summary>
///     Hide the matched elements by fading them to transparent.
///     &#10;1 - fadeOut(duration, callback) 
///     &#10;2 - fadeOut(duration, easing, callback)
/// </summary>
/// <param name="speed" type="Number">
///     A string or number determining how long the animation will run.
/// </param>
/// <param name="easing" type="String">
///     A string indicating which easing function to use for the transition.
/// </param>
/// <param name="callback" type="Function">
///     A function to call once the animation is complete.
/// </param>
/// <returns type="jQuery" />

		return this.animate( props, speed, easing, callback );
	};
jQuery.prototype.fadeTo = function( speed, to, easing, callback ) {
/// <summary>
///     Adjust the opacity of the matched elements.
///     &#10;1 - fadeTo(duration, opacity, callback) 
///     &#10;2 - fadeTo(duration, opacity, easing, callback)
/// </summary>
/// <param name="speed" type="Number">
///     A string or number determining how long the animation will run.
/// </param>
/// <param name="to" type="Number">
///     A number between 0 and 1 denoting the target opacity.
/// </param>
/// <param name="easing" type="String">
///     A string indicating which easing function to use for the transition.
/// </param>
/// <param name="callback" type="Function">
///     A function to call once the animation is complete.
/// </param>
/// <returns type="jQuery" />


		// show any hidden elements after setting opacity to 0
		return this.filter( isHidden ).css( "opacity", 0 ).show()

			// animate to the value specified
			.end().animate({ opacity: to }, speed, easing, callback );
	};
jQuery.prototype.fadeToggle = function( speed, easing, callback ) {
/// <summary>
///     Display or hide the matched elements by animating their opacity.
/// </summary>
/// <param name="speed" type="Number">
///     A string or number determining how long the animation will run.
/// </param>
/// <param name="easing" type="String">
///     A string indicating which easing function to use for the transition.
/// </param>
/// <param name="callback" type="Function">
///     A function to call once the animation is complete.
/// </param>
/// <returns type="jQuery" />

		return this.animate( props, speed, easing, callback );
	};
jQuery.prototype.filter = function( selector ) {
/// <summary>
///     Reduce the set of matched elements to those that match the selector or pass the function's test.
///     &#10;1 - filter(selector) 
///     &#10;2 - filter(function(index)) 
///     &#10;3 - filter(element) 
///     &#10;4 - filter(jQuery object)
/// </summary>
/// <param name="selector" type="String">
///     A string containing a selector expression to match the current set of elements against.
/// </param>
/// <returns type="jQuery" />

		return this.pushStack( winnow(this, selector || [], false) );
	};
jQuery.prototype.find = function( selector ) {
/// <summary>
///     Get the descendants of each element in the current set of matched elements, filtered by a selector, jQuery object, or element.
///     &#10;1 - find(selector) 
///     &#10;2 - find(jQuery object) 
///     &#10;3 - find(element)
/// </summary>
/// <param name="selector" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="jQuery" />

		var i,
			ret = [],
			self = this,
			len = self.length;

		if ( typeof selector !== "string" ) {
			return this.pushStack( jQuery( selector ).filter(function() {
				for ( i = 0; i < len; i++ ) {
					if ( jQuery.contains( self[ i ], this ) ) {
						return true;
					}
				}
			}) );
		}

		for ( i = 0; i < len; i++ ) {
			jQuery.find( selector, self[ i ], ret );
		}

		// Needed because $( selector, context ) becomes $( context ).find( selector )
		ret = this.pushStack( len > 1 ? jQuery.unique( ret ) : ret );
		ret.selector = this.selector ? this.selector + " " + selector : selector;
		return ret;
	};
jQuery.prototype.finish = function( type ) {

		if ( type !== false ) {
			type = type || "fx";
		}
		return this.each(function() {
			var index,
				data = jQuery._data( this ),
				queue = data[ type + "queue" ],
				hooks = data[ type + "queueHooks" ],
				timers = jQuery.timers,
				length = queue ? queue.length : 0;

			// enable finishing flag on private data
			data.finish = true;

			// empty the queue first
			jQuery.queue( this, type, [] );

			if ( hooks && hooks.cur && hooks.cur.finish ) {
				hooks.cur.finish.call( this );
			}

			// look for any active animations, and finish them
			for ( index = timers.length; index--; ) {
				if ( timers[ index ].elem === this && timers[ index ].queue === type ) {
					timers[ index ].anim.stop( true );
					timers.splice( index, 1 );
				}
			}

			// look for any animations in the old queue and finish them
			for ( index = 0; index < length; index++ ) {
				if ( queue[ index ] && queue[ index ].finish ) {
					queue[ index ].finish.call( this );
				}
			}

			// turn off finishing flag
			delete data.finish;
		});
	};
jQuery.prototype.first = function() {
/// <summary>
///     Reduce the set of matched elements to the first in the set.
/// </summary>
/// <returns type="jQuery" />

		return this.eq( 0 );
	};
jQuery.prototype.focus = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "focus" JavaScript event, or trigger that event on an element.
///     &#10;1 - focus(handler(eventObject)) 
///     &#10;2 - focus(eventData, handler(eventObject)) 
///     &#10;3 - focus()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.focusin = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "focusin" event.
///     &#10;1 - focusin(handler(eventObject)) 
///     &#10;2 - focusin(eventData, handler(eventObject))
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.focusout = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "focusout" JavaScript event.
///     &#10;1 - focusout(handler(eventObject)) 
///     &#10;2 - focusout(eventData, handler(eventObject))
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.get = function( num ) {
/// <summary>
///     Retrieve the DOM elements matched by the jQuery object.
/// </summary>
/// <param name="num" type="Number">
///     A zero-based integer indicating which element to retrieve.
/// </param>
/// <returns type="Array" />

		return num == null ?

			// Return a 'clean' array
			this.toArray() :

			// Return just the object
			( num < 0 ? this[ this.length + num ] : this[ num ] );
	};
jQuery.prototype.has = function( target ) {
/// <summary>
///     Reduce the set of matched elements to those that have a descendant that matches the selector or DOM element.
///     &#10;1 - has(selector) 
///     &#10;2 - has(contained)
/// </summary>
/// <param name="target" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="jQuery" />

		var i,
			targets = jQuery( target, this ),
			len = targets.length;

		return this.filter(function() {
			for ( i = 0; i < len; i++ ) {
				if ( jQuery.contains( this, targets[i] ) ) {
					return true;
				}
			}
		});
	};
jQuery.prototype.hasClass = function( selector ) {
/// <summary>
///     Determine whether any of the matched elements are assigned the given class.
/// </summary>
/// <param name="selector" type="String">
///     The class name to search for.
/// </param>
/// <returns type="Boolean" />

		var className = " " + selector + " ",
			i = 0,
			l = this.length;
		for ( ; i < l; i++ ) {
			if ( this[i].nodeType === 1 && (" " + this[i].className + " ").replace(rclass, " ").indexOf( className ) >= 0 ) {
				return true;
			}
		}

		return false;
	};
jQuery.prototype.height = function( margin, value ) {
/// <summary>
///     1: Get the current computed height for the first element in the set of matched elements.
///     &#10;    1.1 - height()
///     &#10;2: Set the CSS height of every matched element.
///     &#10;    2.1 - height(value) 
///     &#10;    2.2 - height(function(index, height))
/// </summary>
/// <param name="margin" type="Number">
///     An integer representing the number of pixels, or an integer with an optional unit of measure appended (as a string).
/// </param>
/// <returns type="jQuery" />

			var chainable = arguments.length && ( defaultExtra || typeof margin !== "boolean" ),
				extra = defaultExtra || ( margin === true || value === true ? "margin" : "border" );

			return jQuery.access( this, function( elem, type, value ) {
				var doc;

				if ( jQuery.isWindow( elem ) ) {
					// As of 5/8/2012 this will yield incorrect results for Mobile Safari, but there
					// isn't a whole lot we can do. See pull request at this URL for discussion:
					// https://github.com/jquery/jquery/pull/764
					return elem.document.documentElement[ "client" + name ];
				}

				// Get document width or height
				if ( elem.nodeType === 9 ) {
					doc = elem.documentElement;

					// Either scroll[Width/Height] or offset[Width/Height] or client[Width/Height], whichever is greatest
					// unfortunately, this causes bug #3838 in IE6/8 only, but there is currently no good, small way to fix it.
					return Math.max(
						elem.body[ "scroll" + name ], doc[ "scroll" + name ],
						elem.body[ "offset" + name ], doc[ "offset" + name ],
						doc[ "client" + name ]
					);
				}

				return value === undefined ?
					// Get width or height on the element, requesting but not forcing parseFloat
					jQuery.css( elem, type, extra ) :

					// Set width or height on the element
					jQuery.style( elem, type, value, extra );
			}, type, chainable ? margin : undefined, chainable, null );
		};
jQuery.prototype.hide = function( speed, easing, callback ) {
/// <summary>
///     Hide the matched elements.
///     &#10;1 - hide() 
///     &#10;2 - hide(duration, callback) 
///     &#10;3 - hide(duration, easing, callback)
/// </summary>
/// <param name="speed" type="Number">
///     A string or number determining how long the animation will run.
/// </param>
/// <param name="easing" type="String">
///     A string indicating which easing function to use for the transition.
/// </param>
/// <param name="callback" type="Function">
///     A function to call once the animation is complete.
/// </param>
/// <returns type="jQuery" />

		return speed == null || typeof speed === "boolean" ?
			cssFn.apply( this, arguments ) :
			this.animate( genFx( name, true ), speed, easing, callback );
	};
jQuery.prototype.hover = function( fnOver, fnOut ) {
/// <summary>
///     1: Bind two handlers to the matched elements, to be executed when the mouse pointer enters and leaves the elements.
///     &#10;    1.1 - hover(handlerIn(eventObject), handlerOut(eventObject))
///     &#10;2: Bind a single handler to the matched elements, to be executed when the mouse pointer enters or leaves the elements.
///     &#10;    2.1 - hover(handlerInOut(eventObject))
/// </summary>
/// <param name="fnOver" type="Function">
///     A function to execute when the mouse pointer enters the element.
/// </param>
/// <param name="fnOut" type="Function">
///     A function to execute when the mouse pointer leaves the element.
/// </param>
/// <returns type="jQuery" />

		return this.mouseenter( fnOver ).mouseleave( fnOut || fnOver );
	};
jQuery.prototype.html = function( value ) {
/// <summary>
///     1: Get the HTML contents of the first element in the set of matched elements.
///     &#10;    1.1 - html()
///     &#10;2: Set the HTML contents of each element in the set of matched elements.
///     &#10;    2.1 - html(htmlString) 
///     &#10;    2.2 - html(function(index, oldhtml))
/// </summary>
/// <param name="value" type="String">
///     A string of HTML to set as the content of each matched element.
/// </param>
/// <returns type="jQuery" />

		return jQuery.access( this, function( value ) {
			var elem = this[0] || {},
				i = 0,
				l = this.length;

			if ( value === undefined ) {
				return elem.nodeType === 1 ?
					elem.innerHTML.replace( rinlinejQuery, "" ) :
					undefined;
			}

			// See if we can take a shortcut and just use innerHTML
			if ( typeof value === "string" && !rnoInnerhtml.test( value ) &&
				( jQuery.support.htmlSerialize || !rnoshimcache.test( value )  ) &&
				( jQuery.support.leadingWhitespace || !rleadingWhitespace.test( value ) ) &&
				!wrapMap[ ( rtagName.exec( value ) || ["", ""] )[1].toLowerCase() ] ) {

				value = value.replace( rxhtmlTag, "<$1></$2>" );

				try {
					for (; i < l; i++ ) {
						// Remove element nodes and prevent memory leaks
						elem = this[i] || {};
						if ( elem.nodeType === 1 ) {
							jQuery.cleanData( getAll( elem, false ) );
							elem.innerHTML = value;
						}
					}

					elem = 0;

				// If using innerHTML throws an exception, use the fallback method
				} catch(e) {}
			}

			if ( elem ) {
				this.empty().append( value );
			}
		}, null, value, arguments.length );
	};
jQuery.prototype.index = function( elem ) {
/// <summary>
///     Search for a given element from among the matched elements.
///     &#10;1 - index() 
///     &#10;2 - index(selector) 
///     &#10;3 - index(element)
/// </summary>
/// <param name="elem" type="String">
///     A selector representing a jQuery collection in which to look for an element.
/// </param>
/// <returns type="Number" />


		// No argument, return index in parent
		if ( !elem ) {
			return ( this[0] && this[0].parentNode ) ? this.first().prevAll().length : -1;
		}

		// index in selector
		if ( typeof elem === "string" ) {
			return jQuery.inArray( this[0], jQuery( elem ) );
		}

		// Locate the position of the desired element
		return jQuery.inArray(
			// If it receives a jQuery object, the first element is used
			elem.jquery ? elem[0] : elem, this );
	};
jQuery.prototype.init = function( selector, context, rootjQuery ) {

		var match, elem;

		// HANDLE: $(""), $(null), $(undefined), $(false)
		if ( !selector ) {
			return this;
		}

		// Handle HTML strings
		if ( typeof selector === "string" ) {
			if ( selector.charAt(0) === "<" && selector.charAt( selector.length - 1 ) === ">" && selector.length >= 3 ) {
				// Assume that strings that start and end with <> are HTML and skip the regex check
				match = [ null, selector, null ];

			} else {
				match = rquickExpr.exec( selector );
			}

			// Match html or make sure no context is specified for #id
			if ( match && (match[1] || !context) ) {

				// HANDLE: $(html) -> $(array)
				if ( match[1] ) {
					context = context instanceof jQuery ? context[0] : context;

					// scripts is true for back-compat
					jQuery.merge( this, jQuery.parseHTML(
						match[1],
						context && context.nodeType ? context.ownerDocument || context : document,
						true
					) );

					// HANDLE: $(html, props)
					if ( rsingleTag.test( match[1] ) && jQuery.isPlainObject( context ) ) {
						for ( match in context ) {
							// Properties of context are called as methods if possible
							if ( jQuery.isFunction( this[ match ] ) ) {
								this[ match ]( context[ match ] );

							// ...and otherwise set as attributes
							} else {
								this.attr( match, context[ match ] );
							}
						}
					}

					return this;

				// HANDLE: $(#id)
				} else {
					elem = document.getElementById( match[2] );

					// Check parentNode to catch when Blackberry 4.6 returns
					// nodes that are no longer in the document #6963
					if ( elem && elem.parentNode ) {
						// Handle the case where IE and Opera return items
						// by name instead of ID
						if ( elem.id !== match[2] ) {
							return rootjQuery.find( selector );
						}

						// Otherwise, we inject the element directly into the jQuery object
						this.length = 1;
						this[0] = elem;
					}

					this.context = document;
					this.selector = selector;
					return this;
				}

			// HANDLE: $(expr, $(...))
			} else if ( !context || context.jquery ) {
				return ( context || rootjQuery ).find( selector );

			// HANDLE: $(expr, context)
			// (which is just equivalent to: $(context).find(expr)
			} else {
				return this.constructor( context ).find( selector );
			}

		// HANDLE: $(DOMElement)
		} else if ( selector.nodeType ) {
			this.context = this[0] = selector;
			this.length = 1;
			return this;

		// HANDLE: $(function)
		// Shortcut for document ready
		} else if ( jQuery.isFunction( selector ) ) {
			return rootjQuery.ready( selector );
		}

		if ( selector.selector !== undefined ) {
			this.selector = selector.selector;
			this.context = selector.context;
		}

		return jQuery.makeArray( selector, this );
	};
jQuery.prototype.innerHeight = function( margin, value ) {
/// <summary>
///     Get the current computed height for the first element in the set of matched elements, including padding but not border.
/// </summary>
/// <returns type="Number" />

			var chainable = arguments.length && ( defaultExtra || typeof margin !== "boolean" ),
				extra = defaultExtra || ( margin === true || value === true ? "margin" : "border" );

			return jQuery.access( this, function( elem, type, value ) {
				var doc;

				if ( jQuery.isWindow( elem ) ) {
					// As of 5/8/2012 this will yield incorrect results for Mobile Safari, but there
					// isn't a whole lot we can do. See pull request at this URL for discussion:
					// https://github.com/jquery/jquery/pull/764
					return elem.document.documentElement[ "client" + name ];
				}

				// Get document width or height
				if ( elem.nodeType === 9 ) {
					doc = elem.documentElement;

					// Either scroll[Width/Height] or offset[Width/Height] or client[Width/Height], whichever is greatest
					// unfortunately, this causes bug #3838 in IE6/8 only, but there is currently no good, small way to fix it.
					return Math.max(
						elem.body[ "scroll" + name ], doc[ "scroll" + name ],
						elem.body[ "offset" + name ], doc[ "offset" + name ],
						doc[ "client" + name ]
					);
				}

				return value === undefined ?
					// Get width or height on the element, requesting but not forcing parseFloat
					jQuery.css( elem, type, extra ) :

					// Set width or height on the element
					jQuery.style( elem, type, value, extra );
			}, type, chainable ? margin : undefined, chainable, null );
		};
jQuery.prototype.innerWidth = function( margin, value ) {
/// <summary>
///     Get the current computed width for the first element in the set of matched elements, including padding but not border.
/// </summary>
/// <returns type="Number" />

			var chainable = arguments.length && ( defaultExtra || typeof margin !== "boolean" ),
				extra = defaultExtra || ( margin === true || value === true ? "margin" : "border" );

			return jQuery.access( this, function( elem, type, value ) {
				var doc;

				if ( jQuery.isWindow( elem ) ) {
					// As of 5/8/2012 this will yield incorrect results for Mobile Safari, but there
					// isn't a whole lot we can do. See pull request at this URL for discussion:
					// https://github.com/jquery/jquery/pull/764
					return elem.document.documentElement[ "client" + name ];
				}

				// Get document width or height
				if ( elem.nodeType === 9 ) {
					doc = elem.documentElement;

					// Either scroll[Width/Height] or offset[Width/Height] or client[Width/Height], whichever is greatest
					// unfortunately, this causes bug #3838 in IE6/8 only, but there is currently no good, small way to fix it.
					return Math.max(
						elem.body[ "scroll" + name ], doc[ "scroll" + name ],
						elem.body[ "offset" + name ], doc[ "offset" + name ],
						doc[ "client" + name ]
					);
				}

				return value === undefined ?
					// Get width or height on the element, requesting but not forcing parseFloat
					jQuery.css( elem, type, extra ) :

					// Set width or height on the element
					jQuery.style( elem, type, value, extra );
			}, type, chainable ? margin : undefined, chainable, null );
		};
jQuery.prototype.insertAfter = function( selector ) {
/// <summary>
///     Insert every element in the set of matched elements after the target.
/// </summary>
/// <param name="selector" type="jQuery">
///     A selector, element, HTML string, or jQuery object; the matched set of elements will be inserted after the element(s) specified by this parameter.
/// </param>
/// <returns type="jQuery" />

		var elems,
			i = 0,
			ret = [],
			insert = jQuery( selector ),
			last = insert.length - 1;

		for ( ; i <= last; i++ ) {
			elems = i === last ? this : this.clone(true);
			jQuery( insert[i] )[ original ]( elems );

			// Modern browsers can apply jQuery collections as arrays, but oldIE needs a .get()
			core_push.apply( ret, elems.get() );
		}

		return this.pushStack( ret );
	};
jQuery.prototype.insertBefore = function( selector ) {
/// <summary>
///     Insert every element in the set of matched elements before the target.
/// </summary>
/// <param name="selector" type="jQuery">
///     A selector, element, HTML string, or jQuery object; the matched set of elements will be inserted before the element(s) specified by this parameter.
/// </param>
/// <returns type="jQuery" />

		var elems,
			i = 0,
			ret = [],
			insert = jQuery( selector ),
			last = insert.length - 1;

		for ( ; i <= last; i++ ) {
			elems = i === last ? this : this.clone(true);
			jQuery( insert[i] )[ original ]( elems );

			// Modern browsers can apply jQuery collections as arrays, but oldIE needs a .get()
			core_push.apply( ret, elems.get() );
		}

		return this.pushStack( ret );
	};
jQuery.prototype.is = function( selector ) {
/// <summary>
///     Check the current matched set of elements against a selector, element, or jQuery object and return true if at least one of these elements matches the given arguments.
///     &#10;1 - is(selector) 
///     &#10;2 - is(function(index)) 
///     &#10;3 - is(jQuery object) 
///     &#10;4 - is(element)
/// </summary>
/// <param name="selector" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="Boolean" />

		return !!winnow(
			this,

			// If this is a positional/relative selector, check membership in the returned set
			// so $("p:first").is("p:last") won't return true for a doc with two "p".
			typeof selector === "string" && rneedsContext.test( selector ) ?
				jQuery( selector ) :
				selector || [],
			false
		).length;
	};
jQuery.prototype.keydown = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "keydown" JavaScript event, or trigger that event on an element.
///     &#10;1 - keydown(handler(eventObject)) 
///     &#10;2 - keydown(eventData, handler(eventObject)) 
///     &#10;3 - keydown()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.keypress = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "keypress" JavaScript event, or trigger that event on an element.
///     &#10;1 - keypress(handler(eventObject)) 
///     &#10;2 - keypress(eventData, handler(eventObject)) 
///     &#10;3 - keypress()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.keyup = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "keyup" JavaScript event, or trigger that event on an element.
///     &#10;1 - keyup(handler(eventObject)) 
///     &#10;2 - keyup(eventData, handler(eventObject)) 
///     &#10;3 - keyup()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.last = function() {
/// <summary>
///     Reduce the set of matched elements to the final one in the set.
/// </summary>
/// <returns type="jQuery" />

		return this.eq( -1 );
	};
jQuery.prototype.length = 0;
jQuery.prototype.load = function( url, params, callback ) {
/// <summary>
///     1: Bind an event handler to the "load" JavaScript event.
///     &#10;    1.1 - load(handler(eventObject)) 
///     &#10;    1.2 - load(eventData, handler(eventObject))
///     &#10;2: Load data from the server and place the returned HTML into the matched element.
///     &#10;    2.1 - load(url, data, complete(responseText, textStatus, XMLHttpRequest))
/// </summary>
/// <param name="url" type="String">
///     A string containing the URL to which the request is sent.
/// </param>
/// <param name="params" type="String">
///     A map or string that is sent to the server with the request.
/// </param>
/// <param name="callback" type="Function">
///     A callback function that is executed when the request completes.
/// </param>
/// <returns type="jQuery" />

	if ( typeof url !== "string" && _load ) {
		return _load.apply( this, arguments );
	}

	var selector, response, type,
		self = this,
		off = url.indexOf(" ");

	if ( off >= 0 ) {
		selector = url.slice( off, url.length );
		url = url.slice( 0, off );
	}

	// If it's a function
	if ( jQuery.isFunction( params ) ) {

		// We assume that it's the callback
		callback = params;
		params = undefined;

	// Otherwise, build a param string
	} else if ( params && typeof params === "object" ) {
		type = "POST";
	}

	// If we have elements to modify, make the request
	if ( self.length > 0 ) {
		jQuery.ajax({
			url: url,

			// if "type" variable is undefined, then "GET" method will be used
			type: type,
			dataType: "html",
			data: params
		}).done(function( responseText ) {

			// Save response for use in complete callback
			response = arguments;

			self.html( selector ?

				// If a selector was specified, locate the right elements in a dummy div
				// Exclude scripts to avoid IE 'Permission Denied' errors
				jQuery("<div>").append( jQuery.parseHTML( responseText ) ).find( selector ) :

				// Otherwise use the full result
				responseText );

		}).complete( callback && function( jqXHR, status ) {
			self.each( callback, response || [ jqXHR.responseText, status, jqXHR ] );
		});
	}

	return this;
};
jQuery.prototype.map = function( callback ) {
/// <summary>
///     Pass each element in the current matched set through a function, producing a new jQuery object containing the return values.
/// </summary>
/// <param name="callback" type="Function">
///     A function object that will be invoked for each element in the current set.
/// </param>
/// <returns type="jQuery" />

		return this.pushStack( jQuery.map(this, function( elem, i ) {
			return callback.call( elem, i, elem );
		}));
	};
jQuery.prototype.mousedown = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "mousedown" JavaScript event, or trigger that event on an element.
///     &#10;1 - mousedown(handler(eventObject)) 
///     &#10;2 - mousedown(eventData, handler(eventObject)) 
///     &#10;3 - mousedown()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.mouseenter = function( data, fn ) {
/// <summary>
///     Bind an event handler to be fired when the mouse enters an element, or trigger that handler on an element.
///     &#10;1 - mouseenter(handler(eventObject)) 
///     &#10;2 - mouseenter(eventData, handler(eventObject)) 
///     &#10;3 - mouseenter()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.mouseleave = function( data, fn ) {
/// <summary>
///     Bind an event handler to be fired when the mouse leaves an element, or trigger that handler on an element.
///     &#10;1 - mouseleave(handler(eventObject)) 
///     &#10;2 - mouseleave(eventData, handler(eventObject)) 
///     &#10;3 - mouseleave()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.mousemove = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "mousemove" JavaScript event, or trigger that event on an element.
///     &#10;1 - mousemove(handler(eventObject)) 
///     &#10;2 - mousemove(eventData, handler(eventObject)) 
///     &#10;3 - mousemove()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.mouseout = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "mouseout" JavaScript event, or trigger that event on an element.
///     &#10;1 - mouseout(handler(eventObject)) 
///     &#10;2 - mouseout(eventData, handler(eventObject)) 
///     &#10;3 - mouseout()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.mouseover = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "mouseover" JavaScript event, or trigger that event on an element.
///     &#10;1 - mouseover(handler(eventObject)) 
///     &#10;2 - mouseover(eventData, handler(eventObject)) 
///     &#10;3 - mouseover()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.mouseup = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "mouseup" JavaScript event, or trigger that event on an element.
///     &#10;1 - mouseup(handler(eventObject)) 
///     &#10;2 - mouseup(eventData, handler(eventObject)) 
///     &#10;3 - mouseup()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.next = function( until, selector ) {
/// <summary>
///     Get the immediately following sibling of each element in the set of matched elements. If a selector is provided, it retrieves the next sibling only if it matches that selector.
/// </summary>
/// <param name="until" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="jQuery" />

		var ret = jQuery.map( this, fn, until );

		if ( name.slice( -5 ) !== "Until" ) {
			selector = until;
		}

		if ( selector && typeof selector === "string" ) {
			ret = jQuery.filter( selector, ret );
		}

		if ( this.length > 1 ) {
			// Remove duplicates
			if ( !guaranteedUnique[ name ] ) {
				ret = jQuery.unique( ret );
			}

			// Reverse order for parents* and prev-derivatives
			if ( rparentsprev.test( name ) ) {
				ret = ret.reverse();
			}
		}

		return this.pushStack( ret );
	};
jQuery.prototype.nextAll = function( until, selector ) {
/// <summary>
///     Get all following siblings of each element in the set of matched elements, optionally filtered by a selector.
/// </summary>
/// <param name="until" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="jQuery" />

		var ret = jQuery.map( this, fn, until );

		if ( name.slice( -5 ) !== "Until" ) {
			selector = until;
		}

		if ( selector && typeof selector === "string" ) {
			ret = jQuery.filter( selector, ret );
		}

		if ( this.length > 1 ) {
			// Remove duplicates
			if ( !guaranteedUnique[ name ] ) {
				ret = jQuery.unique( ret );
			}

			// Reverse order for parents* and prev-derivatives
			if ( rparentsprev.test( name ) ) {
				ret = ret.reverse();
			}
		}

		return this.pushStack( ret );
	};
jQuery.prototype.nextUntil = function( until, selector ) {
/// <summary>
///     Get all following siblings of each element up to but not including the element matched by the selector, DOM node, or jQuery object passed.
///     &#10;1 - nextUntil(selector, filter) 
///     &#10;2 - nextUntil(element, filter)
/// </summary>
/// <param name="until" type="String">
///     A string containing a selector expression to indicate where to stop matching following sibling elements.
/// </param>
/// <param name="selector" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="jQuery" />

		var ret = jQuery.map( this, fn, until );

		if ( name.slice( -5 ) !== "Until" ) {
			selector = until;
		}

		if ( selector && typeof selector === "string" ) {
			ret = jQuery.filter( selector, ret );
		}

		if ( this.length > 1 ) {
			// Remove duplicates
			if ( !guaranteedUnique[ name ] ) {
				ret = jQuery.unique( ret );
			}

			// Reverse order for parents* and prev-derivatives
			if ( rparentsprev.test( name ) ) {
				ret = ret.reverse();
			}
		}

		return this.pushStack( ret );
	};
jQuery.prototype.not = function( selector ) {
/// <summary>
///     Remove elements from the set of matched elements.
///     &#10;1 - not(selector) 
///     &#10;2 - not(elements) 
///     &#10;3 - not(function(index)) 
///     &#10;4 - not(jQuery object)
/// </summary>
/// <param name="selector" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="jQuery" />

		return this.pushStack( winnow(this, selector || [], true) );
	};
jQuery.prototype.off = function( types, selector, fn ) {
/// <summary>
///     Remove an event handler.
///     &#10;1 - off(events, selector, handler(eventObject)) 
///     &#10;2 - off(events-map, selector)
/// </summary>
/// <param name="types" type="String">
///     One or more space-separated event types and optional namespaces, or just namespaces, such as "click", "keydown.myPlugin", or ".myPlugin".
/// </param>
/// <param name="selector" type="String">
///     A selector which should match the one originally passed to .on() when attaching event handlers.
/// </param>
/// <param name="fn" type="Function">
///     A handler function previously attached for the event(s), or the special value false.
/// </param>
/// <returns type="jQuery" />

		var handleObj, type;
		if ( types && types.preventDefault && types.handleObj ) {
			// ( event )  dispatched jQuery.Event
			handleObj = types.handleObj;
			jQuery( types.delegateTarget ).off(
				handleObj.namespace ? handleObj.origType + "." + handleObj.namespace : handleObj.origType,
				handleObj.selector,
				handleObj.handler
			);
			return this;
		}
		if ( typeof types === "object" ) {
			// ( types-object [, selector] )
			for ( type in types ) {
				this.off( type, selector, types[ type ] );
			}
			return this;
		}
		if ( selector === false || typeof selector === "function" ) {
			// ( types [, fn] )
			fn = selector;
			selector = undefined;
		}
		if ( fn === false ) {
			fn = returnFalse;
		}
		return this.each(function() {
			jQuery.event.remove( this, types, fn, selector );
		});
	};
jQuery.prototype.offset = function( options ) {
/// <summary>
///     1: Get the current coordinates of the first element in the set of matched elements, relative to the document.
///     &#10;    1.1 - offset()
///     &#10;2: Set the current coordinates of every element in the set of matched elements, relative to the document.
///     &#10;    2.1 - offset(coordinates) 
///     &#10;    2.2 - offset(function(index, coords))
/// </summary>
/// <param name="options" type="Object">
///     An object containing the properties top and left, which are integers indicating the new top and left coordinates for the elements.
/// </param>
/// <returns type="jQuery" />

	if ( arguments.length ) {
		return options === undefined ?
			this :
			this.each(function( i ) {
				jQuery.offset.setOffset( this, options, i );
			});
	}

	var docElem, win,
		box = { top: 0, left: 0 },
		elem = this[ 0 ],
		doc = elem && elem.ownerDocument;

	if ( !doc ) {
		return;
	}

	docElem = doc.documentElement;

	// Make sure it's not a disconnected DOM node
	if ( !jQuery.contains( docElem, elem ) ) {
		return box;
	}

	// If we don't have gBCR, just use 0,0 rather than error
	// BlackBerry 5, iOS 3 (original iPhone)
	if ( typeof elem.getBoundingClientRect !== core_strundefined ) {
		box = elem.getBoundingClientRect();
	}
	win = getWindow( doc );
	return {
		top: box.top  + ( win.pageYOffset || docElem.scrollTop )  - ( docElem.clientTop  || 0 ),
		left: box.left + ( win.pageXOffset || docElem.scrollLeft ) - ( docElem.clientLeft || 0 )
	};
};
jQuery.prototype.offsetParent = function() {
/// <summary>
///     Get the closest ancestor element that is positioned.
/// </summary>
/// <returns type="jQuery" />

		return this.map(function() {
			var offsetParent = this.offsetParent || docElem;
			while ( offsetParent && ( !jQuery.nodeName( offsetParent, "html" ) && jQuery.css( offsetParent, "position") === "static" ) ) {
				offsetParent = offsetParent.offsetParent;
			}
			return offsetParent || docElem;
		});
	};
jQuery.prototype.on = function( types, selector, data, fn, /*INTERNAL*/ one ) {
/// <summary>
///     Attach an event handler function for one or more events to the selected elements.
///     &#10;1 - on(events, selector, data, handler(eventObject)) 
///     &#10;2 - on(events-map, selector, data)
/// </summary>
/// <param name="types" type="String">
///     One or more space-separated event types and optional namespaces, such as "click" or "keydown.myPlugin".
/// </param>
/// <param name="selector" type="String">
///     A selector string to filter the descendants of the selected elements that trigger the event. If the selector is null or omitted, the event is always triggered when it reaches the selected element.
/// </param>
/// <param name="data" type="Anything">
///     Data to be passed to the handler in event.data when an event is triggered.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute when the event is triggered. The value false is also allowed as a shorthand for a function that simply does return false.
/// </param>
/// <returns type="jQuery" />

		var type, origFn;

		// Types can be a map of types/handlers
		if ( typeof types === "object" ) {
			// ( types-Object, selector, data )
			if ( typeof selector !== "string" ) {
				// ( types-Object, data )
				data = data || selector;
				selector = undefined;
			}
			for ( type in types ) {
				this.on( type, selector, data, types[ type ], one );
			}
			return this;
		}

		if ( data == null && fn == null ) {
			// ( types, fn )
			fn = selector;
			data = selector = undefined;
		} else if ( fn == null ) {
			if ( typeof selector === "string" ) {
				// ( types, selector, fn )
				fn = data;
				data = undefined;
			} else {
				// ( types, data, fn )
				fn = data;
				data = selector;
				selector = undefined;
			}
		}
		if ( fn === false ) {
			fn = returnFalse;
		} else if ( !fn ) {
			return this;
		}

		if ( one === 1 ) {
			origFn = fn;
			fn = function( event ) {
				// Can use an empty set, since event contains the info
				jQuery().off( event );
				return origFn.apply( this, arguments );
			};
			// Use same guid so caller can remove using origFn
			fn.guid = origFn.guid || ( origFn.guid = jQuery.guid++ );
		}
		return this.each( function() {
			jQuery.event.add( this, types, fn, data, selector );
		});
	};
jQuery.prototype.one = function( types, selector, data, fn ) {
/// <summary>
///     Attach a handler to an event for the elements. The handler is executed at most once per element.
///     &#10;1 - one(events, data, handler(eventObject)) 
///     &#10;2 - one(events, selector, data, handler(eventObject)) 
///     &#10;3 - one(events-map, selector, data)
/// </summary>
/// <param name="types" type="String">
///     One or more space-separated event types and optional namespaces, such as "click" or "keydown.myPlugin".
/// </param>
/// <param name="selector" type="String">
///     A selector string to filter the descendants of the selected elements that trigger the event. If the selector is null or omitted, the event is always triggered when it reaches the selected element.
/// </param>
/// <param name="data" type="Anything">
///     Data to be passed to the handler in event.data when an event is triggered.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute when the event is triggered. The value false is also allowed as a shorthand for a function that simply does return false.
/// </param>
/// <returns type="jQuery" />

		return this.on( types, selector, data, fn, 1 );
	};
jQuery.prototype.outerHeight = function( margin, value ) {
/// <summary>
///     Get the current computed height for the first element in the set of matched elements, including padding, border, and optionally margin. Returns an integer (without "px") representation of the value or null if called on an empty set of elements.
/// </summary>
/// <param name="margin" type="Boolean">
///     A Boolean indicating whether to include the element's margin in the calculation.
/// </param>
/// <returns type="Number" />

			var chainable = arguments.length && ( defaultExtra || typeof margin !== "boolean" ),
				extra = defaultExtra || ( margin === true || value === true ? "margin" : "border" );

			return jQuery.access( this, function( elem, type, value ) {
				var doc;

				if ( jQuery.isWindow( elem ) ) {
					// As of 5/8/2012 this will yield incorrect results for Mobile Safari, but there
					// isn't a whole lot we can do. See pull request at this URL for discussion:
					// https://github.com/jquery/jquery/pull/764
					return elem.document.documentElement[ "client" + name ];
				}

				// Get document width or height
				if ( elem.nodeType === 9 ) {
					doc = elem.documentElement;

					// Either scroll[Width/Height] or offset[Width/Height] or client[Width/Height], whichever is greatest
					// unfortunately, this causes bug #3838 in IE6/8 only, but there is currently no good, small way to fix it.
					return Math.max(
						elem.body[ "scroll" + name ], doc[ "scroll" + name ],
						elem.body[ "offset" + name ], doc[ "offset" + name ],
						doc[ "client" + name ]
					);
				}

				return value === undefined ?
					// Get width or height on the element, requesting but not forcing parseFloat
					jQuery.css( elem, type, extra ) :

					// Set width or height on the element
					jQuery.style( elem, type, value, extra );
			}, type, chainable ? margin : undefined, chainable, null );
		};
jQuery.prototype.outerWidth = function( margin, value ) {
/// <summary>
///     Get the current computed width for the first element in the set of matched elements, including padding and border.
/// </summary>
/// <param name="margin" type="Boolean">
///     A Boolean indicating whether to include the element's margin in the calculation.
/// </param>
/// <returns type="Number" />

			var chainable = arguments.length && ( defaultExtra || typeof margin !== "boolean" ),
				extra = defaultExtra || ( margin === true || value === true ? "margin" : "border" );

			return jQuery.access( this, function( elem, type, value ) {
				var doc;

				if ( jQuery.isWindow( elem ) ) {
					// As of 5/8/2012 this will yield incorrect results for Mobile Safari, but there
					// isn't a whole lot we can do. See pull request at this URL for discussion:
					// https://github.com/jquery/jquery/pull/764
					return elem.document.documentElement[ "client" + name ];
				}

				// Get document width or height
				if ( elem.nodeType === 9 ) {
					doc = elem.documentElement;

					// Either scroll[Width/Height] or offset[Width/Height] or client[Width/Height], whichever is greatest
					// unfortunately, this causes bug #3838 in IE6/8 only, but there is currently no good, small way to fix it.
					return Math.max(
						elem.body[ "scroll" + name ], doc[ "scroll" + name ],
						elem.body[ "offset" + name ], doc[ "offset" + name ],
						doc[ "client" + name ]
					);
				}

				return value === undefined ?
					// Get width or height on the element, requesting but not forcing parseFloat
					jQuery.css( elem, type, extra ) :

					// Set width or height on the element
					jQuery.style( elem, type, value, extra );
			}, type, chainable ? margin : undefined, chainable, null );
		};
jQuery.prototype.parent = function( until, selector ) {
/// <summary>
///     Get the parent of each element in the current set of matched elements, optionally filtered by a selector.
/// </summary>
/// <param name="until" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="jQuery" />

		var ret = jQuery.map( this, fn, until );

		if ( name.slice( -5 ) !== "Until" ) {
			selector = until;
		}

		if ( selector && typeof selector === "string" ) {
			ret = jQuery.filter( selector, ret );
		}

		if ( this.length > 1 ) {
			// Remove duplicates
			if ( !guaranteedUnique[ name ] ) {
				ret = jQuery.unique( ret );
			}

			// Reverse order for parents* and prev-derivatives
			if ( rparentsprev.test( name ) ) {
				ret = ret.reverse();
			}
		}

		return this.pushStack( ret );
	};
jQuery.prototype.parents = function( until, selector ) {
/// <summary>
///     Get the ancestors of each element in the current set of matched elements, optionally filtered by a selector.
/// </summary>
/// <param name="until" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="jQuery" />

		var ret = jQuery.map( this, fn, until );

		if ( name.slice( -5 ) !== "Until" ) {
			selector = until;
		}

		if ( selector && typeof selector === "string" ) {
			ret = jQuery.filter( selector, ret );
		}

		if ( this.length > 1 ) {
			// Remove duplicates
			if ( !guaranteedUnique[ name ] ) {
				ret = jQuery.unique( ret );
			}

			// Reverse order for parents* and prev-derivatives
			if ( rparentsprev.test( name ) ) {
				ret = ret.reverse();
			}
		}

		return this.pushStack( ret );
	};
jQuery.prototype.parentsUntil = function( until, selector ) {
/// <summary>
///     Get the ancestors of each element in the current set of matched elements, up to but not including the element matched by the selector, DOM node, or jQuery object.
///     &#10;1 - parentsUntil(selector, filter) 
///     &#10;2 - parentsUntil(element, filter)
/// </summary>
/// <param name="until" type="String">
///     A string containing a selector expression to indicate where to stop matching ancestor elements.
/// </param>
/// <param name="selector" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="jQuery" />

		var ret = jQuery.map( this, fn, until );

		if ( name.slice( -5 ) !== "Until" ) {
			selector = until;
		}

		if ( selector && typeof selector === "string" ) {
			ret = jQuery.filter( selector, ret );
		}

		if ( this.length > 1 ) {
			// Remove duplicates
			if ( !guaranteedUnique[ name ] ) {
				ret = jQuery.unique( ret );
			}

			// Reverse order for parents* and prev-derivatives
			if ( rparentsprev.test( name ) ) {
				ret = ret.reverse();
			}
		}

		return this.pushStack( ret );
	};
jQuery.prototype.position = function() {
/// <summary>
///     Get the current coordinates of the first element in the set of matched elements, relative to the offset parent.
/// </summary>
/// <returns type="Object" />

		if ( !this[ 0 ] ) {
			return;
		}

		var offsetParent, offset,
			parentOffset = { top: 0, left: 0 },
			elem = this[ 0 ];

		// fixed elements are offset from window (parentOffset = {top:0, left: 0}, because it is it's only offset parent
		if ( jQuery.css( elem, "position" ) === "fixed" ) {
			// we assume that getBoundingClientRect is available when computed position is fixed
			offset = elem.getBoundingClientRect();
		} else {
			// Get *real* offsetParent
			offsetParent = this.offsetParent();

			// Get correct offsets
			offset = this.offset();
			if ( !jQuery.nodeName( offsetParent[ 0 ], "html" ) ) {
				parentOffset = offsetParent.offset();
			}

			// Add offsetParent borders
			parentOffset.top  += jQuery.css( offsetParent[ 0 ], "borderTopWidth", true );
			parentOffset.left += jQuery.css( offsetParent[ 0 ], "borderLeftWidth", true );
		}

		// Subtract parent offsets and element margins
		// note: when an element has margin: auto the offsetLeft and marginLeft
		// are the same in Safari causing offset.left to incorrectly be 0
		return {
			top:  offset.top  - parentOffset.top - jQuery.css( elem, "marginTop", true ),
			left: offset.left - parentOffset.left - jQuery.css( elem, "marginLeft", true)
		};
	};
jQuery.prototype.prepend = function() {
/// <summary>
///     Insert content, specified by the parameter, to the beginning of each element in the set of matched elements.
///     &#10;1 - prepend(content, content) 
///     &#10;2 - prepend(function(index, html))
/// </summary>
/// <param name="" type="jQuery">
///     DOM element, array of elements, HTML string, or jQuery object to insert at the beginning of each element in the set of matched elements.
/// </param>
/// <param name="" type="jQuery">
///     One or more additional DOM elements, arrays of elements, HTML strings, or jQuery objects to insert at the beginning of each element in the set of matched elements.
/// </param>
/// <returns type="jQuery" />

		return this.domManip( arguments, function( elem ) {
			if ( this.nodeType === 1 || this.nodeType === 11 || this.nodeType === 9 ) {
				var target = manipulationTarget( this, elem );
				target.insertBefore( elem, target.firstChild );
			}
		});
	};
jQuery.prototype.prependTo = function( selector ) {
/// <summary>
///     Insert every element in the set of matched elements to the beginning of the target.
/// </summary>
/// <param name="selector" type="jQuery">
///     A selector, element, HTML string, or jQuery object; the matched set of elements will be inserted at the beginning of the element(s) specified by this parameter.
/// </param>
/// <returns type="jQuery" />

		var elems,
			i = 0,
			ret = [],
			insert = jQuery( selector ),
			last = insert.length - 1;

		for ( ; i <= last; i++ ) {
			elems = i === last ? this : this.clone(true);
			jQuery( insert[i] )[ original ]( elems );

			// Modern browsers can apply jQuery collections as arrays, but oldIE needs a .get()
			core_push.apply( ret, elems.get() );
		}

		return this.pushStack( ret );
	};
jQuery.prototype.prev = function( until, selector ) {
/// <summary>
///     Get the immediately preceding sibling of each element in the set of matched elements, optionally filtered by a selector.
/// </summary>
/// <param name="until" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="jQuery" />

		var ret = jQuery.map( this, fn, until );

		if ( name.slice( -5 ) !== "Until" ) {
			selector = until;
		}

		if ( selector && typeof selector === "string" ) {
			ret = jQuery.filter( selector, ret );
		}

		if ( this.length > 1 ) {
			// Remove duplicates
			if ( !guaranteedUnique[ name ] ) {
				ret = jQuery.unique( ret );
			}

			// Reverse order for parents* and prev-derivatives
			if ( rparentsprev.test( name ) ) {
				ret = ret.reverse();
			}
		}

		return this.pushStack( ret );
	};
jQuery.prototype.prevAll = function( until, selector ) {
/// <summary>
///     Get all preceding siblings of each element in the set of matched elements, optionally filtered by a selector.
/// </summary>
/// <param name="until" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="jQuery" />

		var ret = jQuery.map( this, fn, until );

		if ( name.slice( -5 ) !== "Until" ) {
			selector = until;
		}

		if ( selector && typeof selector === "string" ) {
			ret = jQuery.filter( selector, ret );
		}

		if ( this.length > 1 ) {
			// Remove duplicates
			if ( !guaranteedUnique[ name ] ) {
				ret = jQuery.unique( ret );
			}

			// Reverse order for parents* and prev-derivatives
			if ( rparentsprev.test( name ) ) {
				ret = ret.reverse();
			}
		}

		return this.pushStack( ret );
	};
jQuery.prototype.prevUntil = function( until, selector ) {
/// <summary>
///     Get all preceding siblings of each element up to but not including the element matched by the selector, DOM node, or jQuery object.
///     &#10;1 - prevUntil(selector, filter) 
///     &#10;2 - prevUntil(element, filter)
/// </summary>
/// <param name="until" type="String">
///     A string containing a selector expression to indicate where to stop matching preceding sibling elements.
/// </param>
/// <param name="selector" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="jQuery" />

		var ret = jQuery.map( this, fn, until );

		if ( name.slice( -5 ) !== "Until" ) {
			selector = until;
		}

		if ( selector && typeof selector === "string" ) {
			ret = jQuery.filter( selector, ret );
		}

		if ( this.length > 1 ) {
			// Remove duplicates
			if ( !guaranteedUnique[ name ] ) {
				ret = jQuery.unique( ret );
			}

			// Reverse order for parents* and prev-derivatives
			if ( rparentsprev.test( name ) ) {
				ret = ret.reverse();
			}
		}

		return this.pushStack( ret );
	};
jQuery.prototype.promise = function( type, obj ) {
/// <summary>
///     Return a Promise object to observe when all actions of a certain type bound to the collection, queued or not, have finished.
/// </summary>
/// <param name="type" type="String">
///     The type of queue that needs to be observed.
/// </param>
/// <param name="obj" type="Object">
///     Object onto which the promise methods have to be attached
/// </param>
/// <returns type="Promise" />

		var tmp,
			count = 1,
			defer = jQuery.Deferred(),
			elements = this,
			i = this.length,
			resolve = function() {
				if ( !( --count ) ) {
					defer.resolveWith( elements, [ elements ] );
				}
			};

		if ( typeof type !== "string" ) {
			obj = type;
			type = undefined;
		}
		type = type || "fx";

		while( i-- ) {
			tmp = jQuery._data( elements[ i ], type + "queueHooks" );
			if ( tmp && tmp.empty ) {
				count++;
				tmp.empty.add( resolve );
			}
		}
		resolve();
		return defer.promise( obj );
	};
jQuery.prototype.prop = function( name, value ) {
/// <summary>
///     1: Get the value of a property for the first element in the set of matched elements.
///     &#10;    1.1 - prop(propertyName)
///     &#10;2: Set one or more properties for the set of matched elements.
///     &#10;    2.1 - prop(propertyName, value) 
///     &#10;    2.2 - prop(map) 
///     &#10;    2.3 - prop(propertyName, function(index, oldPropertyValue))
/// </summary>
/// <param name="name" type="String">
///     The name of the property to set.
/// </param>
/// <param name="value" type="Boolean">
///     A value to set for the property.
/// </param>
/// <returns type="jQuery" />

		return jQuery.access( this, jQuery.prop, name, value, arguments.length > 1 );
	};
jQuery.prototype.pushStack = function( elems ) {
/// <summary>
///     Add a collection of DOM elements onto the jQuery stack.
///     &#10;1 - pushStack(elements) 
///     &#10;2 - pushStack(elements, name, arguments)
/// </summary>
/// <param name="elems" type="Array">
///     An array of elements to push onto the stack and make into a new jQuery object.
/// </param>
/// <param name="" type="String">
///     The name of a jQuery method that generated the array of elements.
/// </param>
/// <param name="" type="Array">
///     The arguments that were passed in to the jQuery method (for serialization).
/// </param>
/// <returns type="jQuery" />


		// Build a new jQuery matched element set
		var ret = jQuery.merge( this.constructor(), elems );

		// Add the old object onto the stack (as a reference)
		ret.prevObject = this;
		ret.context = this.context;

		// Return the newly-formed element set
		return ret;
	};
jQuery.prototype.queue = function( type, data ) {
/// <summary>
///     1: Show the queue of functions to be executed on the matched elements.
///     &#10;    1.1 - queue(queueName)
///     &#10;2: Manipulate the queue of functions to be executed on the matched elements.
///     &#10;    2.1 - queue(queueName, newQueue) 
///     &#10;    2.2 - queue(queueName, callback( next ))
/// </summary>
/// <param name="type" type="String">
///     A string containing the name of the queue. Defaults to fx, the standard effects queue.
/// </param>
/// <param name="data" type="Array">
///     An array of functions to replace the current queue contents.
/// </param>
/// <returns type="jQuery" />

		var setter = 2;

		if ( typeof type !== "string" ) {
			data = type;
			type = "fx";
			setter--;
		}

		if ( arguments.length < setter ) {
			return jQuery.queue( this[0], type );
		}

		return data === undefined ?
			this :
			this.each(function() {
				var queue = jQuery.queue( this, type, data );

				// ensure a hooks for this queue
				jQuery._queueHooks( this, type );

				if ( type === "fx" && queue[0] !== "inprogress" ) {
					jQuery.dequeue( this, type );
				}
			});
	};
jQuery.prototype.ready = function( fn ) {
/// <summary>
///     Specify a function to execute when the DOM is fully loaded.
/// </summary>
/// <param name="fn" type="Function">
///     A function to execute after the DOM is ready.
/// </param>
/// <returns type="jQuery" />

		// Add the callback
		jQuery.ready.promise().done( fn );

		return this;
	};
jQuery.prototype.remove = function( selector, keepData ) {
/// <summary>
///     Remove the set of matched elements from the DOM.
/// </summary>
/// <param name="selector" type="String">
///     A selector expression that filters the set of matched elements to be removed.
/// </param>
/// <returns type="jQuery" />

		var elem,
			elems = selector ? jQuery.filter( selector, this ) : this,
			i = 0;

		for ( ; (elem = elems[i]) != null; i++ ) {

			if ( !keepData && elem.nodeType === 1 ) {
				jQuery.cleanData( getAll( elem ) );
			}

			if ( elem.parentNode ) {
				if ( keepData && jQuery.contains( elem.ownerDocument, elem ) ) {
					setGlobalEval( getAll( elem, "script" ) );
				}
				elem.parentNode.removeChild( elem );
			}
		}

		return this;
	};
jQuery.prototype.removeAttr = function( name ) {
/// <summary>
///     Remove an attribute from each element in the set of matched elements.
/// </summary>
/// <param name="name" type="String">
///     An attribute to remove; as of version 1.7, it can be a space-separated list of attributes.
/// </param>
/// <returns type="jQuery" />

		return this.each(function() {
			jQuery.removeAttr( this, name );
		});
	};
jQuery.prototype.removeClass = function( value ) {
/// <summary>
///     Remove a single class, multiple classes, or all classes from each element in the set of matched elements.
///     &#10;1 - removeClass(className) 
///     &#10;2 - removeClass(function(index, class))
/// </summary>
/// <param name="value" type="String">
///     One or more space-separated classes to be removed from the class attribute of each matched element.
/// </param>
/// <returns type="jQuery" />

		var classes, elem, cur, clazz, j,
			i = 0,
			len = this.length,
			proceed = arguments.length === 0 || typeof value === "string" && value;

		if ( jQuery.isFunction( value ) ) {
			return this.each(function( j ) {
				jQuery( this ).removeClass( value.call( this, j, this.className ) );
			});
		}
		if ( proceed ) {
			classes = ( value || "" ).match( core_rnotwhite ) || [];

			for ( ; i < len; i++ ) {
				elem = this[ i ];
				// This expression is here for better compressibility (see addClass)
				cur = elem.nodeType === 1 && ( elem.className ?
					( " " + elem.className + " " ).replace( rclass, " " ) :
					""
				);

				if ( cur ) {
					j = 0;
					while ( (clazz = classes[j++]) ) {
						// Remove *all* instances
						while ( cur.indexOf( " " + clazz + " " ) >= 0 ) {
							cur = cur.replace( " " + clazz + " ", " " );
						}
					}
					elem.className = value ? jQuery.trim( cur ) : "";
				}
			}
		}

		return this;
	};
jQuery.prototype.removeData = function( key ) {
/// <summary>
///     Remove a previously-stored piece of data.
///     &#10;1 - removeData(name) 
///     &#10;2 - removeData(list)
/// </summary>
/// <param name="key" type="String">
///     A string naming the piece of data to delete.
/// </param>
/// <returns type="jQuery" />

		return this.each(function() {
			jQuery.removeData( this, key );
		});
	};
jQuery.prototype.removeProp = function( name ) {
/// <summary>
///     Remove a property for the set of matched elements.
/// </summary>
/// <param name="name" type="String">
///     The name of the property to set.
/// </param>
/// <returns type="jQuery" />

		name = jQuery.propFix[ name ] || name;
		return this.each(function() {
			// try/catch handles cases where IE balks (such as removing a property on window)
			try {
				this[ name ] = undefined;
				delete this[ name ];
			} catch( e ) {}
		});
	};
jQuery.prototype.replaceAll = function( selector ) {
/// <summary>
///     Replace each target element with the set of matched elements.
/// </summary>
/// <param name="selector" type="String">
///     A selector expression indicating which element(s) to replace.
/// </param>
/// <returns type="jQuery" />

		var elems,
			i = 0,
			ret = [],
			insert = jQuery( selector ),
			last = insert.length - 1;

		for ( ; i <= last; i++ ) {
			elems = i === last ? this : this.clone(true);
			jQuery( insert[i] )[ original ]( elems );

			// Modern browsers can apply jQuery collections as arrays, but oldIE needs a .get()
			core_push.apply( ret, elems.get() );
		}

		return this.pushStack( ret );
	};
jQuery.prototype.replaceWith = function() {
/// <summary>
///     Replace each element in the set of matched elements with the provided new content.
///     &#10;1 - replaceWith(newContent) 
///     &#10;2 - replaceWith(function)
/// </summary>
/// <param name="" type="jQuery">
///     The content to insert. May be an HTML string, DOM element, or jQuery object.
/// </param>
/// <returns type="jQuery" />

		var
			// Snapshot the DOM in case .domManip sweeps something relevant into its fragment
			args = jQuery.map( this, function( elem ) {
				return [ elem.nextSibling, elem.parentNode ];
			}),
			i = 0;

		// Make the changes, replacing each context element with the new content
		this.domManip( arguments, function( elem ) {
			var next = args[ i++ ],
				parent = args[ i++ ];

			if ( parent ) {
				// Don't use the snapshot next if it has moved (#13810)
				if ( next && next.parentNode !== parent ) {
					next = this.nextSibling;
				}
				jQuery( this ).remove();
				parent.insertBefore( elem, next );
			}
		// Allow new content to include elements from the context set
		}, true );

		// Force removal if there was no new content (e.g., from empty arguments)
		return i ? this : this.remove();
	};
jQuery.prototype.resize = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "resize" JavaScript event, or trigger that event on an element.
///     &#10;1 - resize(handler(eventObject)) 
///     &#10;2 - resize(eventData, handler(eventObject)) 
///     &#10;3 - resize()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.scroll = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "scroll" JavaScript event, or trigger that event on an element.
///     &#10;1 - scroll(handler(eventObject)) 
///     &#10;2 - scroll(eventData, handler(eventObject)) 
///     &#10;3 - scroll()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.scrollLeft = function( val ) {
/// <summary>
///     1: Get the current horizontal position of the scroll bar for the first element in the set of matched elements.
///     &#10;    1.1 - scrollLeft()
///     &#10;2: Set the current horizontal position of the scroll bar for each of the set of matched elements.
///     &#10;    2.1 - scrollLeft(value)
/// </summary>
/// <param name="val" type="Number">
///     An integer indicating the new position to set the scroll bar to.
/// </param>
/// <returns type="jQuery" />

		return jQuery.access( this, function( elem, method, val ) {
			var win = getWindow( elem );

			if ( val === undefined ) {
				return win ? (prop in win) ? win[ prop ] :
					win.document.documentElement[ method ] :
					elem[ method ];
			}

			if ( win ) {
				win.scrollTo(
					!top ? val : jQuery( win ).scrollLeft(),
					top ? val : jQuery( win ).scrollTop()
				);

			} else {
				elem[ method ] = val;
			}
		}, method, val, arguments.length, null );
	};
jQuery.prototype.scrollTop = function( val ) {
/// <summary>
///     1: Get the current vertical position of the scroll bar for the first element in the set of matched elements.
///     &#10;    1.1 - scrollTop()
///     &#10;2: Set the current vertical position of the scroll bar for each of the set of matched elements.
///     &#10;    2.1 - scrollTop(value)
/// </summary>
/// <param name="val" type="Number">
///     An integer indicating the new position to set the scroll bar to.
/// </param>
/// <returns type="jQuery" />

		return jQuery.access( this, function( elem, method, val ) {
			var win = getWindow( elem );

			if ( val === undefined ) {
				return win ? (prop in win) ? win[ prop ] :
					win.document.documentElement[ method ] :
					elem[ method ];
			}

			if ( win ) {
				win.scrollTo(
					!top ? val : jQuery( win ).scrollLeft(),
					top ? val : jQuery( win ).scrollTop()
				);

			} else {
				elem[ method ] = val;
			}
		}, method, val, arguments.length, null );
	};
jQuery.prototype.select = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "select" JavaScript event, or trigger that event on an element.
///     &#10;1 - select(handler(eventObject)) 
///     &#10;2 - select(eventData, handler(eventObject)) 
///     &#10;3 - select()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.serialize = function() {
/// <summary>
///     Encode a set of form elements as a string for submission.
/// </summary>
/// <returns type="String" />

		return jQuery.param( this.serializeArray() );
	};
jQuery.prototype.serializeArray = function() {
/// <summary>
///     Encode a set of form elements as an array of names and values.
/// </summary>
/// <returns type="Array" />

		return this.map(function(){
			// Can add propHook for "elements" to filter or add form elements
			var elements = jQuery.prop( this, "elements" );
			return elements ? jQuery.makeArray( elements ) : this;
		})
		.filter(function(){
			var type = this.type;
			// Use .is(":disabled") so that fieldset[disabled] works
			return this.name && !jQuery( this ).is( ":disabled" ) &&
				rsubmittable.test( this.nodeName ) && !rsubmitterTypes.test( type ) &&
				( this.checked || !manipulation_rcheckableType.test( type ) );
		})
		.map(function( i, elem ){
			var val = jQuery( this ).val();

			return val == null ?
				null :
				jQuery.isArray( val ) ?
					jQuery.map( val, function( val ){
						return { name: elem.name, value: val.replace( rCRLF, "\r\n" ) };
					}) :
					{ name: elem.name, value: val.replace( rCRLF, "\r\n" ) };
		}).get();
	};
jQuery.prototype.show = function( speed, easing, callback ) {
/// <summary>
///     Display the matched elements.
///     &#10;1 - show() 
///     &#10;2 - show(duration, callback) 
///     &#10;3 - show(duration, easing, callback)
/// </summary>
/// <param name="speed" type="Number">
///     A string or number determining how long the animation will run.
/// </param>
/// <param name="easing" type="String">
///     A string indicating which easing function to use for the transition.
/// </param>
/// <param name="callback" type="Function">
///     A function to call once the animation is complete.
/// </param>
/// <returns type="jQuery" />

		return speed == null || typeof speed === "boolean" ?
			cssFn.apply( this, arguments ) :
			this.animate( genFx( name, true ), speed, easing, callback );
	};
jQuery.prototype.siblings = function( until, selector ) {
/// <summary>
///     Get the siblings of each element in the set of matched elements, optionally filtered by a selector.
/// </summary>
/// <param name="until" type="String">
///     A string containing a selector expression to match elements against.
/// </param>
/// <returns type="jQuery" />

		var ret = jQuery.map( this, fn, until );

		if ( name.slice( -5 ) !== "Until" ) {
			selector = until;
		}

		if ( selector && typeof selector === "string" ) {
			ret = jQuery.filter( selector, ret );
		}

		if ( this.length > 1 ) {
			// Remove duplicates
			if ( !guaranteedUnique[ name ] ) {
				ret = jQuery.unique( ret );
			}

			// Reverse order for parents* and prev-derivatives
			if ( rparentsprev.test( name ) ) {
				ret = ret.reverse();
			}
		}

		return this.pushStack( ret );
	};
jQuery.prototype.size = function() {
/// <summary>
///     Return the number of elements in the jQuery object.
/// </summary>
/// <returns type="Number" />

	return this.length;
};
jQuery.prototype.slice = function() {
/// <summary>
///     Reduce the set of matched elements to a subset specified by a range of indices.
/// </summary>
/// <param name="" type="Number">
///     An integer indicating the 0-based position at which the elements begin to be selected. If negative, it indicates an offset from the end of the set.
/// </param>
/// <param name="" type="Number">
///     An integer indicating the 0-based position at which the elements stop being selected. If negative, it indicates an offset from the end of the set. If omitted, the range continues until the end of the set.
/// </param>
/// <returns type="jQuery" />

		return this.pushStack( core_slice.apply( this, arguments ) );
	};
jQuery.prototype.slideDown = function( speed, easing, callback ) {
/// <summary>
///     Display the matched elements with a sliding motion.
///     &#10;1 - slideDown(duration, callback) 
///     &#10;2 - slideDown(duration, easing, callback)
/// </summary>
/// <param name="speed" type="Number">
///     A string or number determining how long the animation will run.
/// </param>
/// <param name="easing" type="String">
///     A string indicating which easing function to use for the transition.
/// </param>
/// <param name="callback" type="Function">
///     A function to call once the animation is complete.
/// </param>
/// <returns type="jQuery" />

		return this.animate( props, speed, easing, callback );
	};
jQuery.prototype.slideToggle = function( speed, easing, callback ) {
/// <summary>
///     Display or hide the matched elements with a sliding motion.
///     &#10;1 - slideToggle(duration, callback) 
///     &#10;2 - slideToggle(duration, easing, callback)
/// </summary>
/// <param name="speed" type="Number">
///     A string or number determining how long the animation will run.
/// </param>
/// <param name="easing" type="String">
///     A string indicating which easing function to use for the transition.
/// </param>
/// <param name="callback" type="Function">
///     A function to call once the animation is complete.
/// </param>
/// <returns type="jQuery" />

		return this.animate( props, speed, easing, callback );
	};
jQuery.prototype.slideUp = function( speed, easing, callback ) {
/// <summary>
///     Hide the matched elements with a sliding motion.
///     &#10;1 - slideUp(duration, callback) 
///     &#10;2 - slideUp(duration, easing, callback)
/// </summary>
/// <param name="speed" type="Number">
///     A string or number determining how long the animation will run.
/// </param>
/// <param name="easing" type="String">
///     A string indicating which easing function to use for the transition.
/// </param>
/// <param name="callback" type="Function">
///     A function to call once the animation is complete.
/// </param>
/// <returns type="jQuery" />

		return this.animate( props, speed, easing, callback );
	};
jQuery.prototype.stop = function( type, clearQueue, gotoEnd ) {
/// <summary>
///     Stop the currently-running animation on the matched elements.
///     &#10;1 - stop(clearQueue, jumpToEnd) 
///     &#10;2 - stop(queue, clearQueue, jumpToEnd)
/// </summary>
/// <param name="type" type="String">
///     The name of the queue in which to stop animations.
/// </param>
/// <param name="clearQueue" type="Boolean">
///     A Boolean indicating whether to remove queued animation as well. Defaults to false.
/// </param>
/// <param name="gotoEnd" type="Boolean">
///     A Boolean indicating whether to complete the current animation immediately. Defaults to false.
/// </param>
/// <returns type="jQuery" />

		var stopQueue = function( hooks ) {
			var stop = hooks.stop;
			delete hooks.stop;
			stop( gotoEnd );
		};

		if ( typeof type !== "string" ) {
			gotoEnd = clearQueue;
			clearQueue = type;
			type = undefined;
		}
		if ( clearQueue && type !== false ) {
			this.queue( type || "fx", [] );
		}

		return this.each(function() {
			var dequeue = true,
				index = type != null && type + "queueHooks",
				timers = jQuery.timers,
				data = jQuery._data( this );

			if ( index ) {
				if ( data[ index ] && data[ index ].stop ) {
					stopQueue( data[ index ] );
				}
			} else {
				for ( index in data ) {
					if ( data[ index ] && data[ index ].stop && rrun.test( index ) ) {
						stopQueue( data[ index ] );
					}
				}
			}

			for ( index = timers.length; index--; ) {
				if ( timers[ index ].elem === this && (type == null || timers[ index ].queue === type) ) {
					timers[ index ].anim.stop( gotoEnd );
					dequeue = false;
					timers.splice( index, 1 );
				}
			}

			// start the next in the queue if the last step wasn't forced
			// timers currently will call their complete callbacks, which will dequeue
			// but only if they were gotoEnd
			if ( dequeue || !gotoEnd ) {
				jQuery.dequeue( this, type );
			}
		});
	};
jQuery.prototype.submit = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "submit" JavaScript event, or trigger that event on an element.
///     &#10;1 - submit(handler(eventObject)) 
///     &#10;2 - submit(eventData, handler(eventObject)) 
///     &#10;3 - submit()
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.text = function( value ) {
/// <summary>
///     1: Get the combined text contents of each element in the set of matched elements, including their descendants.
///     &#10;    1.1 - text()
///     &#10;2: Set the content of each element in the set of matched elements to the specified text.
///     &#10;    2.1 - text(textString) 
///     &#10;    2.2 - text(function(index, text))
/// </summary>
/// <param name="value" type="String">
///     A string of text to set as the content of each matched element.
/// </param>
/// <returns type="jQuery" />

		return jQuery.access( this, function( value ) {
			return value === undefined ?
				jQuery.text( this ) :
				this.empty().append( ( this[0] && this[0].ownerDocument || document ).createTextNode( value ) );
		}, null, value, arguments.length );
	};
jQuery.prototype.toArray = function() {
/// <summary>
///     Retrieve all the DOM elements contained in the jQuery set, as an array.
/// </summary>
/// <returns type="Array" />

		return core_slice.call( this );
	};
jQuery.prototype.toggle = function( speed, easing, callback ) {
/// <summary>
///     1: Bind two or more handlers to the matched elements, to be executed on alternate clicks.
///     &#10;    1.1 - toggle(handler(eventObject), handler(eventObject), handler(eventObject))
///     &#10;2: Display or hide the matched elements.
///     &#10;    2.1 - toggle(duration, callback) 
///     &#10;    2.2 - toggle(duration, easing, callback) 
///     &#10;    2.3 - toggle(showOrHide)
/// </summary>
/// <param name="speed" type="Function">
///     A function to execute every even time the element is clicked.
/// </param>
/// <param name="easing" type="Function">
///     A function to execute every odd time the element is clicked.
/// </param>
/// <param name="callback" type="Function">
///     Additional handlers to cycle through after clicks.
/// </param>
/// <returns type="jQuery" />

		return speed == null || typeof speed === "boolean" ?
			cssFn.apply( this, arguments ) :
			this.animate( genFx( name, true ), speed, easing, callback );
	};
jQuery.prototype.toggleClass = function( value, stateVal ) {
/// <summary>
///     Add or remove one or more classes from each element in the set of matched elements, depending on either the class's presence or the value of the switch argument.
///     &#10;1 - toggleClass(className) 
///     &#10;2 - toggleClass(className, switch) 
///     &#10;3 - toggleClass(switch) 
///     &#10;4 - toggleClass(function(index, class, switch), switch)
/// </summary>
/// <param name="value" type="String">
///     One or more class names (separated by spaces) to be toggled for each element in the matched set.
/// </param>
/// <param name="stateVal" type="Boolean">
///     A Boolean (not just truthy/falsy) value to determine whether the class should be added or removed.
/// </param>
/// <returns type="jQuery" />

		var type = typeof value,
			isBool = typeof stateVal === "boolean";

		if ( jQuery.isFunction( value ) ) {
			return this.each(function( i ) {
				jQuery( this ).toggleClass( value.call(this, i, this.className, stateVal), stateVal );
			});
		}

		return this.each(function() {
			if ( type === "string" ) {
				// toggle individual class names
				var className,
					i = 0,
					self = jQuery( this ),
					state = stateVal,
					classNames = value.match( core_rnotwhite ) || [];

				while ( (className = classNames[ i++ ]) ) {
					// check each className given, space separated list
					state = isBool ? state : !self.hasClass( className );
					self[ state ? "addClass" : "removeClass" ]( className );
				}

			// Toggle whole class name
			} else if ( type === core_strundefined || type === "boolean" ) {
				if ( this.className ) {
					// store className if set
					jQuery._data( this, "__className__", this.className );
				}

				// If the element has a class name or if we're passed "false",
				// then remove the whole classname (if there was one, the above saved it).
				// Otherwise bring back whatever was previously saved (if anything),
				// falling back to the empty string if nothing was stored.
				this.className = this.className || value === false ? "" : jQuery._data( this, "__className__" ) || "";
			}
		});
	};
jQuery.prototype.trigger = function( type, data ) {
/// <summary>
///     Execute all handlers and behaviors attached to the matched elements for the given event type.
///     &#10;1 - trigger(eventType, extraParameters) 
///     &#10;2 - trigger(event)
/// </summary>
/// <param name="type" type="String">
///     A string containing a JavaScript event type, such as click or submit.
/// </param>
/// <param name="data" type="Object">
///     Additional parameters to pass along to the event handler.
/// </param>
/// <returns type="jQuery" />

		return this.each(function() {
			jQuery.event.trigger( type, data, this );
		});
	};
jQuery.prototype.triggerHandler = function( type, data ) {
/// <summary>
///     Execute all handlers attached to an element for an event.
/// </summary>
/// <param name="type" type="String">
///     A string containing a JavaScript event type, such as click or submit.
/// </param>
/// <param name="data" type="Array">
///     An array of additional parameters to pass along to the event handler.
/// </param>
/// <returns type="Object" />

		var elem = this[0];
		if ( elem ) {
			return jQuery.event.trigger( type, data, elem, true );
		}
	};
jQuery.prototype.unbind = function( types, fn ) {
/// <summary>
///     Remove a previously-attached event handler from the elements.
///     &#10;1 - unbind(eventType, handler(eventObject)) 
///     &#10;2 - unbind(eventType, false) 
///     &#10;3 - unbind(event)
/// </summary>
/// <param name="types" type="String">
///     A string containing a JavaScript event type, such as click or submit.
/// </param>
/// <param name="fn" type="Function">
///     The function that is to be no longer executed.
/// </param>
/// <returns type="jQuery" />

		return this.off( types, null, fn );
	};
jQuery.prototype.undelegate = function( selector, types, fn ) {
/// <summary>
///     Remove a handler from the event for all elements which match the current selector, based upon a specific set of root elements.
///     &#10;1 - undelegate() 
///     &#10;2 - undelegate(selector, eventType) 
///     &#10;3 - undelegate(selector, eventType, handler(eventObject)) 
///     &#10;4 - undelegate(selector, events) 
///     &#10;5 - undelegate(namespace)
/// </summary>
/// <param name="selector" type="String">
///     A selector which will be used to filter the event results.
/// </param>
/// <param name="types" type="String">
///     A string containing a JavaScript event type, such as "click" or "keydown"
/// </param>
/// <param name="fn" type="Function">
///     A function to execute at the time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		// ( namespace ) or ( selector, types [, fn] )
		return arguments.length === 1 ? this.off( selector, "**" ) : this.off( types, selector || "**", fn );
	};
jQuery.prototype.unload = function( data, fn ) {
/// <summary>
///     Bind an event handler to the "unload" JavaScript event.
///     &#10;1 - unload(handler(eventObject)) 
///     &#10;2 - unload(eventData, handler(eventObject))
/// </summary>
/// <param name="data" type="Object">
///     A map of data that will be passed to the event handler.
/// </param>
/// <param name="fn" type="Function">
///     A function to execute each time the event is triggered.
/// </param>
/// <returns type="jQuery" />

		return arguments.length > 0 ?
			this.on( name, null, data, fn ) :
			this.trigger( name );
	};
jQuery.prototype.unwrap = function() {
/// <summary>
///     Remove the parents of the set of matched elements from the DOM, leaving the matched elements in their place.
/// </summary>
/// <returns type="jQuery" />

		return this.parent().each(function() {
			if ( !jQuery.nodeName( this, "body" ) ) {
				jQuery( this ).replaceWith( this.childNodes );
			}
		}).end();
	};
jQuery.prototype.val = function( value ) {
/// <summary>
///     1: Get the current value of the first element in the set of matched elements.
///     &#10;    1.1 - val()
///     &#10;2: Set the value of each element in the set of matched elements.
///     &#10;    2.1 - val(value) 
///     &#10;    2.2 - val(function(index, value))
/// </summary>
/// <param name="value" type="String">
///     A string of text or an array of strings corresponding to the value of each matched element to set as selected/checked.
/// </param>
/// <returns type="jQuery" />

		var ret, hooks, isFunction,
			elem = this[0];

		if ( !arguments.length ) {
			if ( elem ) {
				hooks = jQuery.valHooks[ elem.type ] || jQuery.valHooks[ elem.nodeName.toLowerCase() ];

				if ( hooks && "get" in hooks && (ret = hooks.get( elem, "value" )) !== undefined ) {
					return ret;
				}

				ret = elem.value;

				return typeof ret === "string" ?
					// handle most common string cases
					ret.replace(rreturn, "") :
					// handle cases where value is null/undef or number
					ret == null ? "" : ret;
			}

			return;
		}

		isFunction = jQuery.isFunction( value );

		return this.each(function( i ) {
			var val;

			if ( this.nodeType !== 1 ) {
				return;
			}

			if ( isFunction ) {
				val = value.call( this, i, jQuery( this ).val() );
			} else {
				val = value;
			}

			// Treat null/undefined as ""; convert numbers to string
			if ( val == null ) {
				val = "";
			} else if ( typeof val === "number" ) {
				val += "";
			} else if ( jQuery.isArray( val ) ) {
				val = jQuery.map(val, function ( value ) {
					return value == null ? "" : value + "";
				});
			}

			hooks = jQuery.valHooks[ this.type ] || jQuery.valHooks[ this.nodeName.toLowerCase() ];

			// If set returns undefined, fall back to normal setting
			if ( !hooks || !("set" in hooks) || hooks.set( this, val, "value" ) === undefined ) {
				this.value = val;
			}
		});
	};
jQuery.prototype.width = function( margin, value ) {
/// <summary>
///     1: Get the current computed width for the first element in the set of matched elements.
///     &#10;    1.1 - width()
///     &#10;2: Set the CSS width of each element in the set of matched elements.
///     &#10;    2.1 - width(value) 
///     &#10;    2.2 - width(function(index, width))
/// </summary>
/// <param name="margin" type="Number">
///     An integer representing the number of pixels, or an integer along with an optional unit of measure appended (as a string).
/// </param>
/// <returns type="jQuery" />

			var chainable = arguments.length && ( defaultExtra || typeof margin !== "boolean" ),
				extra = defaultExtra || ( margin === true || value === true ? "margin" : "border" );

			return jQuery.access( this, function( elem, type, value ) {
				var doc;

				if ( jQuery.isWindow( elem ) ) {
					// As of 5/8/2012 this will yield incorrect results for Mobile Safari, but there
					// isn't a whole lot we can do. See pull request at this URL for discussion:
					// https://github.com/jquery/jquery/pull/764
					return elem.document.documentElement[ "client" + name ];
				}

				// Get document width or height
				if ( elem.nodeType === 9 ) {
					doc = elem.documentElement;

					// Either scroll[Width/Height] or offset[Width/Height] or client[Width/Height], whichever is greatest
					// unfortunately, this causes bug #3838 in IE6/8 only, but there is currently no good, small way to fix it.
					return Math.max(
						elem.body[ "scroll" + name ], doc[ "scroll" + name ],
						elem.body[ "offset" + name ], doc[ "offset" + name ],
						doc[ "client" + name ]
					);
				}

				return value === undefined ?
					// Get width or height on the element, requesting but not forcing parseFloat
					jQuery.css( elem, type, extra ) :

					// Set width or height on the element
					jQuery.style( elem, type, value, extra );
			}, type, chainable ? margin : undefined, chainable, null );
		};
jQuery.prototype.wrap = function( html ) {
/// <summary>
///     Wrap an HTML structure around each element in the set of matched elements.
///     &#10;1 - wrap(wrappingElement) 
///     &#10;2 - wrap(function(index))
/// </summary>
/// <param name="html" type="jQuery">
///     An HTML snippet, selector expression, jQuery object, or DOM element specifying the structure to wrap around the matched elements.
/// </param>
/// <returns type="jQuery" />

		var isFunction = jQuery.isFunction( html );

		return this.each(function(i) {
			jQuery( this ).wrapAll( isFunction ? html.call(this, i) : html );
		});
	};
jQuery.prototype.wrapAll = function( html ) {
/// <summary>
///     Wrap an HTML structure around all elements in the set of matched elements.
/// </summary>
/// <param name="html" type="jQuery">
///     An HTML snippet, selector expression, jQuery object, or DOM element specifying the structure to wrap around the matched elements.
/// </param>
/// <returns type="jQuery" />

		if ( jQuery.isFunction( html ) ) {
			return this.each(function(i) {
				jQuery(this).wrapAll( html.call(this, i) );
			});
		}

		if ( this[0] ) {
			// The elements to wrap the target around
			var wrap = jQuery( html, this[0].ownerDocument ).eq(0).clone(true);

			if ( this[0].parentNode ) {
				wrap.insertBefore( this[0] );
			}

			wrap.map(function() {
				var elem = this;

				while ( elem.firstChild && elem.firstChild.nodeType === 1 ) {
					elem = elem.firstChild;
				}

				return elem;
			}).append( this );
		}

		return this;
	};
jQuery.prototype.wrapInner = function( html ) {
/// <summary>
///     Wrap an HTML structure around the content of each element in the set of matched elements.
///     &#10;1 - wrapInner(wrappingElement) 
///     &#10;2 - wrapInner(function(index))
/// </summary>
/// <param name="html" type="String">
///     An HTML snippet, selector expression, jQuery object, or DOM element specifying the structure to wrap around the content of the matched elements.
/// </param>
/// <returns type="jQuery" />

		if ( jQuery.isFunction( html ) ) {
			return this.each(function(i) {
				jQuery(this).wrapInner( html.call(this, i) );
			});
		}

		return this.each(function() {
			var self = jQuery( this ),
				contents = self.contents();

			if ( contents.length ) {
				contents.wrapAll( html );

			} else {
				self.append( html );
			}
		});
	};
jQuery.fn = jQuery.prototype;
jQuery.fn.init.prototype = jQuery.fn;
window.jQuery = window.$ = jQuery;
})(window);
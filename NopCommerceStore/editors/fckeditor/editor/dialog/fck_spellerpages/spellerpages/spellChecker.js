////////////////////////////////////////////////////
// spellChecker.js
//
// spellChecker object
//
// This file is sourced on web pages that have a textarea object to evaluate
// for spelling. It includes the implementation for the spellCheckObject.
//
////////////////////////////////////////////////////


// constructor
function spellChecker( textObject ) {

	// public properties - configurable
//	this.popUpUrl = '/speller/spellchecker.html';							// by FredCK
	this.popUpUrl = 'fck_spellerpages/spellerpages/spellchecker.html';		// by FredCK
	this.popUpName = 'spellchecker';
//	this.popUpProps = "menu=no,width=440,height=350,top=70,left=120,resizable=yes,status=yes";	// by FredCK
	this.popUpProps = null ;																	// by FredCK
//	this.spellCheckScript = '/speller/server-scripts/spellchecker.php';		// by FredCK
	//this.spellCheckScript = '/cgi-bin/spellchecker.pl';

	// values used to keep track of what happened to a word
	this.replWordFlag = "R";	// single replace
	this.ignrWordFlag = "I";	// single ignore
	this.replAllFlag = "RA";	// replace all occurances
	this.ignrAllFlag = "IA";	// ignore all occurances
	this.fromReplAll = "~RA";	// an occurance of a "replace all" word
	this.fromIgnrAll = "~IA";	// an occurance of a "ignore all" word
	// properties set at run time
	this.wordFlags = new Array();
	this.currentTextIndex = 0;
	this.currentWordIndex = 0;
	this.spellCheckerWin = null;
	this.controlWin = null;
	this.wordWin = null;
	this.textArea = textObject;	// deprecated
	this.textInputs = arguments;

	// private methods
	this._spellcheck = _spellcheck;
	this._getSuggestions = _getSuggestions;
	this._setAsIgnored = _setAsIgnored;
	this._getTotalReplaced = _getTotalReplaced;
	this._setWordText = _setWordText;
	this._getFormInputs = _getFormInputs;

	// public methods
	this.openChecker = openChecker;
	this.startCheck = startCheck;
	this.checkTextBoxes = checkTextBoxes;
	this.checkTextAreas = checkTextAreas;
	this.spellCheckAll = spellCheckAll;
	this.ignoreWord = ignoreWord;
	this.ignoreAll = ignoreAll;
	this.replaceWord = replaceWord;
	this.replaceAll = replaceAll;
	this.terminateSpell = terminateSpell;
	this.undo = undo;

	// set the current window's "speller" property to the instance of this class.
	// this object can now be referenced by child windows/frames.
	window.speller = this;
}

// call this method to check all text boxes (and only text boxes) in the HTML document
function checkTextBoxes() {
	this.textInputs = this._getFormInputs( "^text$" );
	this.openChecker();
}

// call this method to check all textareas (and only textareas ) in the HTML document
function checkTextAreas() {
	this.textInputs = this._getFormInputs( "^textarea$" );
	this.openChecker();
}

// call this method to check all text boxes and textareas in the HTML document
function spellCheckAll() {
	this.textInputs = this._getFormInputs( "^text(area)?$" );
	this.openChecker();
}

// call this method to check text boxe(s) and/or textarea(s) that were passed in to the
// object's constructor or to the textInputs property
function openChecker() {
	this.spellCheckerWin = window.open( this.popUpUrl, this.popUpName, this.popUpProps );
	if( !this.spellCheckerWin.opener ) {
		this.spellCheckerWin.opener = window;
	}
}

function startCheck( wordWindowObj, controlWindowObj ) {

	// set properties from args
	this.wordWin = wordWindowObj;
	this.controlWin = controlWindowObj;

	// reset properties
	this.wordWin.resetForm();
	this.controlWin.resetForm();
	this.currentTextIndex = 0;
	this.currentWordIndex = 0;
	// initialize the flags to an array - one element for each text input
	this.wordFlags = new Array( this.wordWin.textInputs.length );
	// each element will be an array that keeps track of each word in the text
	for( var i=0; i<this.wordFlags.length; i++ ) {
		this.wordFlags[i] = [];
	}

	// start
	this._spellcheck();

	return true;
}

function ignoreWord() {
	var wi = this.currentWordIndex;
	var ti = this.currentTextIndex;
	if( !this.wordWin ) {
		alert( 'Error: Word frame not available.' );
		return false;
	}
	if( !this.wordWin.getTextVal( ti, wi )) {
		alert( 'Error: "Not in dictionary" text is missing.' );
		return false;
	}
	// set as ignored
	if( this._setAsIgnored( ti, wi, this.ignrWordFlag )) {
		this.currentWordIndex++;
		this._spellcheck();
	}
	return true;
}

function ignoreAll() {
	var wi = this.currentWordIndex;
	var ti = this.currentTextIndex;
	if( !this.wordWin ) {
		alert( 'Error: Word frame not available.' );
		return false;
	}
	// get the word that is currently being evaluated.
	var s_word_to_repl = this.wordWin.getTextVal( ti, wi );
	if( !s_word_to_repl ) {
		alert( 'Error: "Not in dictionary" text is missing' );
		return false;
	}

	// set this word as an "ignore all" word.
	this._setAsIgnored( ti, wi, this.ignrAllFlag );

	// loop through all the words after this word
	for( var i = ti; i < this.wordWin.textInputs.length; i++ ) {
		for( var j = 0; j < this.wordWin.totalWords( i ); j++ ) {
			if(( i == ti && j > wi ) || i > ti ) {
				// future word: set as "from ignore all" if
				// 1) do not already have a flag and
				// 2) have the same value as current word
				if(( this.wordWin.getTextVal( i, j ) == s_word_to_repl )
				&& ( !this.wordFlags[i][j] )) {
					this._setAsIgnored( i, j, this.fromIgnrAll );
				}
			}
		}
	}

	// finally, move on
	this.currentWordIndex++;
	this._spellcheck();
	return true;
}

function replaceWord() {
	var wi = this.currentWordIndex;
	var ti = this.currentTextIndex;
	if( !this.wordWin ) {
		alert( 'Error: Word frame not available.' );
		return false;
	}
	if( !this.wordWin.getTextVal( ti, wi )) {
		alert( 'Error: "Not in dictionary" text is missing' );
		return false;
	}
	if( !this.controlWin.replacementText ) {
		return false ;
	}
	var txt = this.controlWin.replacementText;
	if( txt.value ) {
		var newspell = new String( txt.value );
		if( this._setWordText( ti, wi, newspell, this.replWordFlag )) {
			this.currentWordIndex++;
			this._spellcheck();
		}
	}
	return true;
}

function replaceAll() {
	var ti = this.currentTextIndex;
	var wi = this.currentWordIndex;
	if( !this.wordWin ) {
		alert( 'Error: Word frame not available.' );
		return false;
	}
	var s_word_to_repl = this.wordWin.getTextVal( ti, wi );
	if( !s_word_to_repl ) {
		alert( 'Error: "Not in dictionary" text is missing' );
		return false;
	}
	var txt = this.controlWin.replacementText;
	if( !txt.value ) return false;
	var newspell = new String( txt.value );

	// set this word as a "replace all" word.
	this._setWordText( ti, wi, newspell, this.replAllFlag );

	// loop through all the words after this word
	for( var i = ti; i < this.wordWin.textInputs.length; i++ ) {
		for( var j = 0; j < this.wordWin.totalWords( i ); j++ ) {
			if(( i == ti && j > wi ) || i > ti ) {
				// future word: set word text to s_word_to_repl if
				// 1) do not already have a flag and
				// 2) have the same value as s_word_to_repl
				if(( this.wordWin.getTextVal( i, j ) == s_word_to_repl )
				&& ( !this.wordFlags[i][j] )) {
					this._setWordText( i, j, newspell, this.fromReplAll );
				}
			}
		}
	}

	// finally, move on
	this.currentWordIndex++;
	this._spellcheck();
	return true;
}

function terminateSpell() {
	// called when we have reached the end of the spell checking.
	var msg = "";		// by FredCK
	var numrepl = this._getTotalReplaced();
	if( numrepl == 0 ) {
		// see if there were no misspellings to begin with
		if( !this.wordWin ) {
			msg = "";
		} else {
			if( this.wordWin.totalMisspellings() ) {
//				msg += "No words changed.";			// by FredCK
				msg += FCKLang.DlgSpellNoChanges ;	// by FredCK
			} else {
//				msg += "No misspellings found.";	// by FredCK
				msg += FCKLang.DlgSpellNoMispell ;	// by FredCK
			}
		}
	} else if( numrepl == 1 ) {
//		msg += "One word changed.";			// by FredCK
		msg += FCKLang.DlgSpellOneChange ;	// by FredCK
	} else {
//		msg += numrepl + " words changed.";	// by FredCK
		msg += FCKLang.DlgSpellManyChanges.replace( /%1/g, numrepl ) ;
	}
	if( msg ) {
//		msg += "\n";	// by FredCK
		alert( msg );
	}

	if( numrepl > 0 ) {
		// update the text field(s) on the opener window
		for( var i = 0; i < this.textInputs.length; i++ ) {
			// this.textArea.value = this.wordWin.text;
			if( this.wordWin ) {
				if( this.wordWin.textInputs[i] ) {
					this.textInputs[i].value = this.wordWin.textInputs[i];
				}
			}
		}
	}

	// return back to the calling window
//	this.spellCheckerWin.close();					// by FredCK
	if ( typeof( this.OnFinished ) == 'function' )	// by FredCK
		this.OnFinished(numrepl) ;					// by FredCK

	return true;
}

function undo() {
	// skip if this is the first word!
	var ti = this.currentTextIndex;
	var wi = this.currentWordIndex;

	if( this.wordWin.totalPreviousWords( ti, wi ) > 0 ) {
		this.wordWin.removeFocus( ti, wi );

		// go back to the last word index that was acted upon
		do {
			// if the current word index is zero then reset the seed
			if( this.currentWordIndex == 0 && this.currentTextIndex > 0 ) {
				this.currentTextIndex--;
				this.currentWordIndex = this.wordWin.totalWords( this.currentTextIndex )-1;
				if( this.currentWordIndex < 0 ) this.currentWordIndex = 0;
			} else {
				if( this.currentWordIndex > 0 ) {
					this.currentWordIndex--;
				}
			}
		} while (
			this.wordWin.totalWords( this.currentTextIndex ) == 0
			|| this.wordFlags[this.currentTextIndex][this.currentWordIndex] == this.fromIgnrAll
			|| this.wordFlags[this.currentTextIndex][this.currentWordIndex] == this.fromReplAll
		);

		var text_idx = this.currentTextIndex;
		var idx = this.currentWordIndex;
		var preReplSpell = this.wordWin.originalSpellings[text_idx][idx];

		// if we got back to the first word then set the Undo button back to disabled
		if( this.wordWin.totalPreviousWords( text_idx, idx ) == 0 ) {
			this.controlWin.disableUndo();
		}

		var i, j, origSpell ;
		// examine what happened to this current word.
		switch( this.wordFlags[text_idx][idx] ) {
			// replace all: go through this and all the future occurances of the word
			// and revert them all to the original spelling and clear their flags
			case this.replAllFlag :
				for( i = text_idx; i < this.wordWin.textInputs.length; i++ ) {
					for( j = 0; j < this.wordWin.totalWords( i ); j++ ) {
						if(( i == text_idx && j >= idx ) || i > text_idx ) {
							origSpell = this.wordWin.originalSpellings[i][j];
							if( origSpell == preReplSpell ) {
								this._setWordText ( i, j, origSpell, undefined );
							}
						}
					}
				}
				break;

			// ignore all: go through all the future occurances of the word
			// and clear their flags
			case this.ignrAllFlag :
				for( i = text_idx; i < this.wordWin.textInputs.length; i++ ) {
					for( j = 0; j < this.wordWin.totalWords( i ); j++ ) {
						if(( i == text_idx && j >= idx ) || i > text_idx ) {
							origSpell = this.wordWin.originalSpellings[i][j];
							if( origSpell == preReplSpell ) {
								this.wordFlags[i][j] = undefined;
							}
						}
					}
				}
				break;

			// replace: revert the word to its original spelling
			case this.replWordFlag :
				this._setWordText ( text_idx, idx, preReplSpell, undefined );
				break;
		}

		// For all four cases, clear the wordFlag of this word. re-start the process
		this.wordFlags[text_idx][idx] = undefined;
		this._spellcheck();
	}
}

function _spellcheck() {
	var ww = this.wordWin;

	// check if this is the last word in the current text element
	if( this.currentWordIndex == ww.totalWords( this.currentTextIndex) ) {
		this.currentTextIndex++;
		this.currentWordIndex = 0;
		// keep going if we're not yet past the last text element
		if( this.currentTextIndex < this.wordWin.textInputs.length ) {
			this._spellcheck();
			return;
		} else {
			this.terminateSpell();
			return;
		}
	}

	// if this is after the first one make sure the Undo button is enabled
	if( this.currentWordIndex > 0 ) {
		this.controlWin.enableUndo();
	}

	// skip the current word if it has already been worked on
	if( this.wordFlags[this.currentTextIndex][this.currentWordIndex] ) {
		// increment the global current word index and move on.
		this.currentWordIndex++;
		this._spellcheck();
	} else {
		var evalText = ww.getTextVal( this.currentTextIndex, this.currentWordIndex );
		if( evalText ) {
			this.controlWin.evaluatedText.value = evalText;
			ww.setFocus( this.currentTextIndex, this.currentWordIndex );
			this._getSuggestions( this.currentTextIndex, this.currentWordIndex );
		}
	}
}

function _getSuggestions( text_num, word_num ) {
	this.controlWin.clearSuggestions();
	// add suggestion in list for each suggested word.
	// get the array of suggested words out of the
	// three-dimensional array containing all suggestions.
	var a_suggests = this.wordWin.suggestions[text_num][word_num];
	if( a_suggests ) {
		// got an array of suggestions.
		for( var ii = 0; ii < a_suggests.length; ii++ ) {
			this.controlWin.addSuggestion( a_suggests[ii] );
		}
	}
	this.controlWin.selectDefaultSuggestion();
}

function _setAsIgnored( text_num, word_num, flag ) {
	// set the UI
	this.wordWin.removeFocus( text_num, word_num );
	// do the bookkeeping
	this.wordFlags[text_num][word_num] = flag;
	return true;
}

function _getTotalReplaced() {
	var i_replaced = 0;
	for( var i = 0; i < this.wordFlags.length; i++ ) {
		for( var j = 0; j < this.wordFlags[i].length; j++ ) {
			if(( this.wordFlags[i][j] == this.replWordFlag )
			|| ( this.wordFlags[i][j] == this.replAllFlag )
			|| ( this.wordFlags[i][j] == this.fromReplAll )) {
				i_replaced++;
			}
		}
	}
	return i_replaced;
}

function _setWordText( text_num, word_num, newText, flag ) {
	// set the UI and form inputs
	this.wordWin.setText( text_num, word_num, newText );
	// keep track of what happened to this word:
	this.wordFlags[text_num][word_num] = flag;
	return true;
}

function _getFormInputs( inputPattern ) {
	var inputs = new Array();
	for( var i = 0; i < document.forms.length; i++ ) {
		for( var j = 0; j < document.forms[i].elements.length; j++ ) {
			if( document.forms[i].elements[j].type.match( inputPattern )) {
				inputs[inputs.length] = document.forms[i].elements[j];
			}
		}
	}
	return inputs;
}

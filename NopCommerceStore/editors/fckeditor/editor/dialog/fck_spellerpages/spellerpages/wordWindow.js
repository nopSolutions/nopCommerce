////////////////////////////////////////////////////
// wordWindow object
////////////////////////////////////////////////////
function wordWindow() {
	// private properties
	this._forms = [];

	// private methods
	this._getWordObject = _getWordObject;
	//this._getSpellerObject = _getSpellerObject;
	this._wordInputStr = _wordInputStr;
	this._adjustIndexes = _adjustIndexes;
	this._isWordChar = _isWordChar;
	this._lastPos = _lastPos;

	// public properties
	this.wordChar = /[a-zA-Z]/;
	this.windowType = "wordWindow";
	this.originalSpellings = new Array();
	this.suggestions = new Array();
	this.checkWordBgColor = "pink";
	this.normWordBgColor = "white";
	this.text = "";
	this.textInputs = new Array();
	this.indexes = new Array();
	//this.speller = this._getSpellerObject();

	// public methods
	this.resetForm = resetForm;
	this.totalMisspellings = totalMisspellings;
	this.totalWords = totalWords;
	this.totalPreviousWords = totalPreviousWords;
	//this.getTextObjectArray = getTextObjectArray;
	this.getTextVal = getTextVal;
	this.setFocus = setFocus;
	this.removeFocus = removeFocus;
	this.setText = setText;
	//this.getTotalWords = getTotalWords;
	this.writeBody = writeBody;
	this.printForHtml = printForHtml;
}

function resetForm() {
	if( this._forms ) {
		for( var i = 0; i < this._forms.length; i++ ) {
			this._forms[i].reset();
		}
	}
	return true;
}

function totalMisspellings() {
	var total_words = 0;
	for( var i = 0; i < this.textInputs.length; i++ ) {
		total_words += this.totalWords( i );
	}
	return total_words;
}

function totalWords( textIndex ) {
	return this.originalSpellings[textIndex].length;
}

function totalPreviousWords( textIndex, wordIndex ) {
	var total_words = 0;
	for( var i = 0; i <= textIndex; i++ ) {
		for( var j = 0; j < this.totalWords( i ); j++ ) {
			if( i == textIndex && j == wordIndex ) {
				break;
			} else {
				total_words++;
			}
		}
	}
	return total_words;
}

//function getTextObjectArray() {
//	return this._form.elements;
//}

function getTextVal( textIndex, wordIndex ) {
	var word = this._getWordObject( textIndex, wordIndex );
	if( word ) {
		return word.value;
	}
}

function setFocus( textIndex, wordIndex ) {
	var word = this._getWordObject( textIndex, wordIndex );
	if( word ) {
		if( word.type == "text" ) {
			word.focus();
			word.style.backgroundColor = this.checkWordBgColor;
		}
	}
}

function removeFocus( textIndex, wordIndex ) {
	var word = this._getWordObject( textIndex, wordIndex );
	if( word ) {
		if( word.type == "text" ) {
			word.blur();
			word.style.backgroundColor = this.normWordBgColor;
		}
	}
}

function setText( textIndex, wordIndex, newText ) {
	var word = this._getWordObject( textIndex, wordIndex );
	var beginStr;
	var endStr;
	if( word ) {
		var pos = this.indexes[textIndex][wordIndex];
		var oldText = word.value;
		// update the text given the index of the string
		beginStr = this.textInputs[textIndex].substring( 0, pos );
		endStr = this.textInputs[textIndex].substring(
			pos + oldText.length,
			this.textInputs[textIndex].length
		);
		this.textInputs[textIndex] = beginStr + newText + endStr;

		// adjust the indexes on the stack given the differences in
		// length between the new word and old word.
		var lengthDiff = newText.length - oldText.length;
		this._adjustIndexes( textIndex, wordIndex, lengthDiff );

		word.size = newText.length;
		word.value = newText;
		this.removeFocus( textIndex, wordIndex );
	}
}


function writeBody() {
	var d = window.document;
	var is_html = false;

	d.open();

	// iterate through each text input.
	for( var txtid = 0; txtid < this.textInputs.length; txtid++ ) {
		var end_idx = 0;
		var begin_idx = 0;
		d.writeln( '<form name="textInput'+txtid+'">' );
		var wordtxt = this.textInputs[txtid];
		this.indexes[txtid] = [];

		if( wordtxt ) {
			var orig = this.originalSpellings[txtid];
			if( !orig ) break;

			//!!! plain text, or HTML mode?
			d.writeln( '<div class="plainText">' );
			// iterate through each occurrence of a misspelled word.
			for( var i = 0; i < orig.length; i++ ) {
				// find the position of the current misspelled word,
				// starting at the last misspelled word.
				// and keep looking if it's a substring of another word
				do {
					begin_idx = wordtxt.indexOf( orig[i], end_idx );
					end_idx = begin_idx + orig[i].length;
					// word not found? messed up!
					if( begin_idx == -1 ) break;
					// look at the characters immediately before and after
					// the word. If they are word characters we'll keep looking.
					var before_char = wordtxt.charAt( begin_idx - 1 );
					var after_char = wordtxt.charAt( end_idx );
				} while (
					this._isWordChar( before_char )
					|| this._isWordChar( after_char )
				);

				// keep track of its position in the original text.
				this.indexes[txtid][i] = begin_idx;

				// write out the characters before the current misspelled word
				for( var j = this._lastPos( txtid, i ); j < begin_idx; j++ ) {
					// !!! html mode? make it html compatible
					d.write( this.printForHtml( wordtxt.charAt( j )));
				}

				// write out the misspelled word.
				d.write( this._wordInputStr( orig[i] ));

				// if it's the last word, write out the rest of the text
				if( i == orig.length-1 ){
					d.write( printForHtml( wordtxt.substr( end_idx )));
				}
			}

			d.writeln( '</div>' );

		}
		d.writeln( '</form>' );
	}
	//for ( var j = 0; j < d.forms.length; j++ ) {
	//	alert( d.forms[j].name );
	//	for( var k = 0; k < d.forms[j].elements.length; k++ ) {
	//		alert( d.forms[j].elements[k].name + ": " + d.forms[j].elements[k].value );
	//	}
	//}

	// set the _forms property
	this._forms = d.forms;
	d.close();
}

// return the character index in the full text after the last word we evaluated
function _lastPos( txtid, idx ) {
	if( idx > 0 )
		return this.indexes[txtid][idx-1] + this.originalSpellings[txtid][idx-1].length;
	else
		return 0;
}

function printForHtml( n ) {
	return n ;		// by FredCK
/*
	var htmlstr = n;
	if( htmlstr.length == 1 ) {
		// do simple case statement if it's just one character
		switch ( n ) {
			case "\n":
				htmlstr = '<br/>';
				break;
			case "<":
				htmlstr = '&lt;';
				break;
			case ">":
				htmlstr = '&gt;';
				break;
		}
		return htmlstr;
	} else {
		htmlstr = htmlstr.replace( /</g, '&lt' );
		htmlstr = htmlstr.replace( />/g, '&gt' );
		htmlstr = htmlstr.replace( /\n/g, '<br/>' );
		return htmlstr;
	}
*/
}

function _isWordChar( letter ) {
	if( letter.search( this.wordChar ) == -1 ) {
		return false;
	} else {
		return true;
	}
}

function _getWordObject( textIndex, wordIndex ) {
	if( this._forms[textIndex] ) {
		if( this._forms[textIndex].elements[wordIndex] ) {
			return this._forms[textIndex].elements[wordIndex];
		}
	}
	return null;
}

function _wordInputStr( word ) {
	var str = '<input readonly ';
	str += 'class="blend" type="text" value="' + word + '" size="' + word.length + '">';
	return str;
}

function _adjustIndexes( textIndex, wordIndex, lengthDiff ) {
	for( var i = wordIndex + 1; i < this.originalSpellings[textIndex].length; i++ ) {
		this.indexes[textIndex][i] = this.indexes[textIndex][i] + lengthDiff;
	}
}

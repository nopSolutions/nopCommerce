#!/usr/bin/perl

use CGI qw/ :standard /;
use File::Temp qw/ tempfile tempdir /;

# my $spellercss = '/speller/spellerStyle.css';					# by FredCK
my $spellercss = '../spellerStyle.css';							# by FredCK
# my $wordWindowSrc = '/speller/wordWindow.js';					# by FredCK
my $wordWindowSrc = '../wordWindow.js';							# by FredCK
my @textinputs = param( 'textinputs[]' ); # array
# my $aspell_cmd = 'aspell';									# by FredCK (for Linux)
my $aspell_cmd = '"C:\Program Files\Aspell\bin\aspell.exe"';	# by FredCK (for Windows)
my $lang = 'en_US';
# my $aspell_opts = "-a --lang=$lang --encoding=utf-8";			# by FredCK
my $aspell_opts = "-a --lang=$lang --encoding=utf-8 -H --rem-sgml-check=alt";		# by FredCK
my $input_separator = "A";

# set the 'wordtext' JavaScript variable to the submitted text.
sub printTextVar {
	for( my $i = 0; $i <= $#textinputs; $i++ ) {
	        print "textinputs[$i] = decodeURIComponent('" . escapeQuote( $textinputs[$i] ) . "')\n";
	}
}

sub printTextIdxDecl {
	my $idx = shift;
	print "words[$idx] = [];\n";
	print "suggs[$idx] = [];\n";
}

sub printWordsElem {
	my( $textIdx, $wordIdx, $word ) = @_;
	print "words[$textIdx][$wordIdx] = '" . escapeQuote( $word ) . "';\n";
}

sub printSuggsElem {
	my( $textIdx, $wordIdx, @suggs ) = @_;
	print "suggs[$textIdx][$wordIdx] = [";
	for my $i ( 0..$#suggs ) {
		print "'" . escapeQuote( $suggs[$i] ) . "'";
		if( $i < $#suggs ) {
			print ", ";
		}
	}
	print "];\n";
}

sub printCheckerResults {
	my $textInputIdx = -1;
	my $wordIdx = 0;
	my $unhandledText;
	# create temp file
	my $dir = tempdir( CLEANUP => 1 );
	my( $fh, $tmpfilename ) = tempfile( DIR => $dir );

	# temp file was created properly?

	# open temp file, add the submitted text.
	for( my $i = 0; $i <= $#textinputs; $i++ ) {
		$text = url_decode( $textinputs[$i] );
		# Strip all tags for the text. (by FredCK - #339 / #681)
		$text =~ s/<[^>]+>/ /g;
		@lines = split( /\n/, $text );
		print $fh "\%\n"; # exit terse mode
		print $fh "^$input_separator\n";
		print $fh "!\n";  # enter terse mode
		for my $line ( @lines ) {
			# use carat on each line to escape possible aspell commands
			print $fh "^$line\n";
		}

	}
	# exec aspell command
	my $cmd = "$aspell_cmd $aspell_opts < $tmpfilename 2>&1";
	open ASPELL, "$cmd |" or handleError( "Could not execute `$cmd`\\n$!" ) and return;
	# parse each line of aspell return
	for my $ret ( <ASPELL> ) {
		chomp( $ret );
		# if '&', then not in dictionary but has suggestions
		# if '#', then not in dictionary and no suggestions
		# if '*', then it is a delimiter between text inputs
		if( $ret =~ /^\*/ ) {
			$textInputIdx++;
			printTextIdxDecl( $textInputIdx );
			$wordIdx = 0;

		} elsif( $ret =~ /^(&|#)/ ) {
			my @tokens = split( " ", $ret, 5 );
			printWordsElem( $textInputIdx, $wordIdx, $tokens[1] );
			my @suggs = ();
			if( $tokens[4] ) {
				@suggs = split( ", ", $tokens[4] );
			}
			printSuggsElem( $textInputIdx, $wordIdx, @suggs );
			$wordIdx++;
		} else {
			$unhandledText .= $ret;
		}
	}
	close ASPELL or handleError( "Error executing `$cmd`\\n$unhandledText" ) and return;
}

sub escapeQuote {
	my $str = shift;
	$str =~ s/'/\\'/g;
	return $str;
}

sub handleError {
	my $err = shift;
	print "error = '" . escapeQuote( $err ) . "';\n";
}

sub url_decode {
	local $_ = @_ ? shift : $_;
	defined or return;
	# change + signs to spaces
	tr/+/ /;
	# change hex escapes to the proper characters
	s/%([a-fA-F0-9]{2})/pack "H2", $1/eg;
	return $_;
}

# # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #
# Display HTML
# # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

print <<EOF;
Content-type: text/html; charset=utf-8

<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<link rel="stylesheet" type="text/css" href="$spellercss"/>
<script src="$wordWindowSrc"></script>
<script type="text/javascript">
var suggs = new Array();
var words = new Array();
var textinputs = new Array();
var error;
EOF

printTextVar();

printCheckerResults();

print <<EOF;
var wordWindowObj = new wordWindow();
wordWindowObj.originalSpellings = words;
wordWindowObj.suggestions = suggs;
wordWindowObj.textInputs = textinputs;


function init_spell() {
	// check if any error occured during server-side processing
	if( error ) {
		alert( error );
	} else {
		// call the init_spell() function in the parent frameset
		if (parent.frames.length) {
			parent.init_spell( wordWindowObj );
		} else {
			error = "This page was loaded outside of a frameset. ";
			error += "It might not display properly";
			alert( error );
		}
	}
}

</script>

</head>
<body onLoad="init_spell();">

<script type="text/javascript">
wordWindowObj.writeBody();
</script>

</body>
</html>
EOF

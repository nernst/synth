grammar AudioSynthesisGrammar;

compileUnit
	: finalUnit=expr EOF
	;

exprList
	: expr (',' expr)*
	;

expr
	: '(' inner=expr ')'								# subExpr
	| op=('+' | '-') child=expr							# unaryExpr
	| leftChild=expr op=('*' | '/') rightChild=expr		# binaryExpr
	| leftChild=expr op=('+' | '-') rightChild=expr		# binaryExpr
	| funcName=ID '(' arguments=exprList ')'			# functionExpr
	| value=NUM											# valueExpr
	| PI												# piExpr
	| EULER												# eulerExpr
	| TIME_VAR											# timeVarExpr
	;

OP_ADD: '+';
OP_SUB: '-';
OP_MUL: '*';
OP_DIV: '/';

PI: 'pi';
EULER: 'e';
TIME_VAR: 't';

ID: [a-zA-Z] [_a-zA-Z0-9]*;

NUM: [0-9]+ ('.' [0-9]+)? ([eE] [+-]? [0-9]+ ('.' [0-9]+)? )?;
WS: [ \t\r\n] -> skip;
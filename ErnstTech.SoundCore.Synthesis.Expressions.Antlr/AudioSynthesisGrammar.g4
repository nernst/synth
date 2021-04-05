grammar AudioSynthesisGrammar;

compileUnit
	: finalUnit=expr EOF
	;

exprList
	: expr (',' expr)*
	;

expr
	: L_PAREN inner=expr R_PAREN							# subExpr
	| op=(OP_ADD | OP_SUB) child=expr						# unaryExpr
	| leftChild=expr op=OP_EXP rightChild=expr				# binaryExpr
	| leftChild=expr op=(OP_MUL | OP_DIV) rightChild=expr	# binaryExpr
	| leftChild=expr op=(OP_ADD | OP_SUB) rightChild=expr	# binaryExpr
	| funcName=ID L_PAREN arguments=exprList? R_PAREN		# functionExpr
	| value=NUM												# valueExpr
	| PI													# piExpr
	| EULER													# eulerExpr
	| TIME_VAR												# timeVarExpr
	;

L_PAREN: '(';
R_PAREN: ')';

OP_ADD: '+';
OP_SUB: '-';
OP_MUL: '*';
OP_DIV: '/';
OP_EXP: '^';

PI: 'pi';
EULER: 'e';
TIME_VAR: 't';

ID: [a-zA-Z] [_a-zA-Z0-9]*;

NUM: [0-9]+ ('.' [0-9]+)? ([eE] [+-]? [0-9]+ ('.' [0-9]+)? )?;
WS: [ \t\r\n] -> skip;
use [Shultze]
go
	SELECT o.NAME as OPTION_NAME, 
		   r.OPTION_ID, 
		   r.[PRIORITY], 
		   t.NAME as THEME_NAME, 
		   r.THEME_ID, 
		   r.[SESSION]
	  FROM [OPTION] o
inner join RESPONSE r
		on r.OPTION_ID = o.ID
inner join THEME t
        on t.ID = r.THEME_ID
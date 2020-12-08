

SELECT *
FROM [t訂單] o
WHERE o.f狀態=2 AND DATEADD(day,5,(SELECT TOP 1 value FROM STRING_SPLIT(o.f預定日期,' '))) < (GETDATE()) 

-- SQL 字串分割
(SELECT TOP 1 value FROM STRING_SPLIT(o.f預定日期,' ')) 

-- 選出所有超過5天未評價訂單
SELECT o.fOID [fOID], o.fPID [fPID], c.fCID [fCID]
FROM
(SELECT fOID , fPID 
FROM [t訂單] o
WHERE o.f狀態=2 AND 
DATEADD(day,5,
	(SELECT TOP 1 value FROM STRING_SPLIT(o.f預定日期,' '))
) < GETDATE()
) AS o
JOIN [t販售項目] p ON o.fPID=p.fPID
JOIN [t私廚] c ON p.fCID=c.fCID


-- 3. 依[訂單][數量] * [v評價VM][價格] 加入至私廚的 [會員][點數]
UPDATE u
SET u.f點數= u.f點數 - CAST(ROUND(0.9*o.f數量*p.f價格, 0) AS int)
FROM
	(SELECT *
	FROM [t訂單] o
	WHERE o.fPID=@fPID
		) AS o
JOIN [t販售項目] p ON o.fPID=p.fPID
JOIN [t私廚] c ON p.fCID=c.fCID
JOIN [t會員] u ON c.fUID=u.fUID
 

SELECT *
FROM
	(SELECT *
	FROM [t訂單] o
	WHERE o.f狀態= AND 
		DATEADD(day,5,
			(SELECT TOP 1 value FROM STRING_SPLIT(o.f預定日期,' '))
		) < GETDATE()) AS o
JOIN [t販售項目] p ON o.fPID=p.fPID
JOIN [t私廚] c ON p.fCID=c.fCID
JOIN [t會員] u ON c.fUID=u.fUID
 

-- 2. 變更 [私廚] 的評級 : 由 [訂單] 平均算出
UPDATE c
SET c.f私廚評級 = chef_Avg.平均評級,
	c.f私廚評級筆數 = chef_Avg.總筆數
FROM 
	(SELECT	
		CAST(ROUND(AVG(CAST(o.f評級 AS decimal)),0) AS int) [平均評級], 
		COUNT(*) [總筆數], c.fCID [fCID] 
	FROM [t私廚] c 
	JOIN [t販售項目] p ON c.fCID = p.fCID 
	JOIN [t訂單] o ON o.fPID = p.fPID 
	WHERE c.fCID = @fCID  and o.f狀態 = 3 
	GROUP BY c.fCID) AS chef_Avg
JOIN [t私廚] c ON chef_Avg.fCID=c.fCID
WHERE c.fCID = @fCID


-- 1. 更變訂單狀態 私廚確認_開放客戶評價 -> 客戶確認_完成評價
UPDATE o
SET o.f評價日期=GETDATE(),
	o.f狀態=3,
	o.f評級=5,
	o.f評價內容=N'5天未評價，系統自動評價'
FROM [t訂單] o
WHERE o.fOID=@fOID


SELECT *
FROM [t訂單] o
WHERE o.f狀態 = 3



-- SQL 交易
BEGIN TRAN





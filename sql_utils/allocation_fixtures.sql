INSERT INTO [Funds].[AllocationVersions] ([Name], [Version], [ScoreRange_Min], [ScoreRange_Max]) VALUES (N'100i', 1803, 0, 0)
INSERT INTO [Funds].[AllocationVersions] ([Name], [Version], [ScoreRange_Min], [ScoreRange_Max]) VALUES (N'80i20e', 1803, 1, 10)
INSERT INTO [Funds].[AllocationVersions] ([Name], [Version], [ScoreRange_Min], [ScoreRange_Max]) VALUES (N'70i30e', 1803, 11, 21)
INSERT INTO [Funds].[AllocationVersions] ([Name], [Version], [ScoreRange_Min], [ScoreRange_Max]) VALUES (N'60i40e', 1803, 22, 30)
INSERT INTO [Funds].[AllocationVersions] ([Name], [Version], [ScoreRange_Min], [ScoreRange_Max]) VALUES (N'50i50e', 1803, 31, 59)
INSERT INTO [Funds].[AllocationVersions] ([Name], [Version], [ScoreRange_Min], [ScoreRange_Max]) VALUES (N'40i60e', 1803, 60, 87)
INSERT INTO [Funds].[AllocationVersions] ([Name], [Version], [ScoreRange_Min], [ScoreRange_Max]) VALUES (N'30i70e', 1803, 88, 114)
INSERT INTO [Funds].[AllocationVersions] ([Name], [Version], [ScoreRange_Min], [ScoreRange_Max]) VALUES (N'20i80e', 1803, 115, 135)
INSERT INTO [Funds].[AllocationVersions] ([Name], [Version], [ScoreRange_Min], [ScoreRange_Max]) VALUES (N'100e', 1803, 136, 150)

SET IDENTITY_INSERT [Funds].[Option] ON
INSERT INTO [Funds].[Option] ([Id], [Name], [OptionNumber]) VALUES (1, N'One Fund Option', 1)
INSERT INTO [Funds].[Option] ([Id], [Name], [OptionNumber]) VALUES (2, N'Two Fund Option', 2)
INSERT INTO [Funds].[Option] ([Id], [Name], [OptionNumber]) VALUES (3, N'Three Fund Option', 3)
SET IDENTITY_INSERT [Funds].[Option] OFF


SET IDENTITY_INSERT [Funds].[AllocationOptions] ON
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (1, 1, N'100e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (2, 1, N'20i80e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (3, 1, N'30i70e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (4, 1, N'40i60e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (5, 1, N'50i50e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (6, 1, N'60i40e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (7, 1, N'70i30e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (8, 1, N'80i20e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (9, 1, N'100i', 1803)

INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (10, 2, N'100e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (11, 2, N'20i80e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (12, 2, N'30i70e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (13, 2, N'40i60e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (14, 2, N'50i50e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (15, 2, N'60i40e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (16, 2, N'70i30e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (17, 2, N'80i20e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (18, 2, N'100i', 1803)

INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (19, 3, N'100e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (20, 3, N'20i80e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (21, 3, N'30i70e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (22, 3, N'40i60e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (23, 3, N'50i50e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (24, 3, N'60i40e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (25, 3, N'70i30e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (26, 3, N'80i20e', 1803)
INSERT INTO [Funds].[AllocationOptions] ([Id], [OptionId], [AllocationVersionName], [AllocationVersionVersion]) VALUES (27, 3, N'100i', 1803)
SET IDENTITY_INSERT [Funds].[AllocationOptions] OFF

INSERT INTO [Funds].[Funds] ([FundCode]) VALUES (N'cig4248')
INSERT INTO [Funds].[Funds] ([FundCode]) VALUES (N'cig4247')
INSERT INTO [Funds].[Funds] ([FundCode]) VALUES (N'cig4246')
INSERT INTO [Funds].[Funds] ([FundCode]) VALUES (N'cig4245')
INSERT INTO [Funds].[Funds] ([FundCode]) VALUES (N'cig4244')
INSERT INTO [Funds].[Funds] ([FundCode]) VALUES (N'cig4243')
INSERT INTO [Funds].[Funds] ([FundCode]) VALUES (N'cig4242')
INSERT INTO [Funds].[Funds] ([FundCode]) VALUES (N'cig4241')
INSERT INTO [Funds].[Funds] ([FundCode]) VALUES (N'cig4290')
INSERT INTO [Funds].[Funds] ([FundCode]) VALUES (N'pmo205')
INSERT INTO [Funds].[Funds] ([FundCode]) VALUES (N'edg500')
INSERT INTO [Funds].[Funds] ([FundCode]) VALUES (N'maw104')

SET IDENTITY_INSERT [Funds].[CompositionPart] ON
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (1, 1, 100, N'cig4248')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (2, 2, 100, N'cig4247')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (3, 3, 100, N'cig4246')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (4, 4, 100, N'cig4245')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (5, 5, 100, N'cig4244')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (6, 6, 100, N'cig4243')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (7, 7, 100, N'cig4242')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (8, 8, 100, N'cig4241')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (9, 9, 100, N'cig4290')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (10, 10, 0, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (10, 11, 100, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (11, 12, 20, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (11, 13, 80, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (12, 14, 30, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (12, 15, 70, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (13, 16, 40, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (13, 17, 60, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (14, 18, 50, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (14, 19, 50, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (15, 20, 60, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (15, 21, 40, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (16, 22, 70, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (16, 23, 30, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (17, 24, 80, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (17, 25, 20, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (18, 26, 100, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (18, 27, 0, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (19, 28, 0, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (19, 29, 0, N'maw104')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (19, 30, 100, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (20, 31, 10, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (20, 32, 20, N'maw104')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (20, 33, 70, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (21, 34, 20, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (21, 35, 25, N'maw104')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (21, 36, 55, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (22, 37, 30, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (22, 38, 30, N'maw104')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (22, 39, 40, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (23, 40, 40, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (23, 41, 35, N'maw104')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (23, 42, 25, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (24, 43, 40, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (24, 44, 50, N'maw104')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (24, 45, 10, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (25, 46, 50, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (25, 47, 45, N'maw104')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (25, 48, 5, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (26, 49, 65, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (26, 50, 30, N'maw104')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (26, 51, 5, N'edg500')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (27, 52, 100, N'pmo205')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (27, 53, 0, N'maw104')
INSERT INTO [Funds].[CompositionPart] ([OptionId], [Id], [Percent], [FundCode]) VALUES (27, 54, 0, N'edg500')
SET IDENTITY_INSERT [Funds].[CompositionPart] OFF

INSERT INTO [Funds].[VersionDrafts] ([Version], [Date], [Draft]) VALUES (1803, '20180318', N'<?xml version="1.0" encoding="utf-16"?>
<ArrayOfAllocationDTO xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <AllocationDTO>
    <Name>100i</Name>
    <Version>1803</Version>
    <ScoreRange>
      <Min>0</Min>
      <Max>0</Max>
    </ScoreRange>
    <Options>
      <AllocationOptionDTO>
        <Name>One Fund Option</Name>
        <OptionNumber>1</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>100</Percent>
            <FundCode>cig4290</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Two Fund Option</Name>
        <OptionNumber>2</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>100</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>0</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Three Fund Option</Name>
        <OptionNumber>3</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>100</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>0</Percent>
            <FundCode>maw104</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>0</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
    </Options>
  </AllocationDTO>
  <AllocationDTO>
    <Name>80i20e</Name>
    <Version>1803</Version>
    <ScoreRange>
      <Min>1</Min>
      <Max>10</Max>
    </ScoreRange>
    <Options>
      <AllocationOptionDTO>
        <Name>One Fund Option</Name>
        <OptionNumber>1</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>100</Percent>
            <FundCode>cig4241</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Two Fund Option</Name>
        <OptionNumber>2</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>80</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>20</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Three Fund Option</Name>
        <OptionNumber>3</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>65</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>30</Percent>
            <FundCode>maw104</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>5</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
    </Options>
  </AllocationDTO>
  <AllocationDTO>
    <Name>70i30e</Name>
    <Version>1803</Version>
    <ScoreRange>
      <Min>11</Min>
      <Max>21</Max>
    </ScoreRange>
    <Options>
      <AllocationOptionDTO>
        <Name>One Fund Option</Name>
        <OptionNumber>1</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>100</Percent>
            <FundCode>cig4242</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Two Fund Option</Name>
        <OptionNumber>2</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>70</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>30</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Three Fund Option</Name>
        <OptionNumber>3</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>50</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>45</Percent>
            <FundCode>maw104</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>5</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
    </Options>
  </AllocationDTO>
  <AllocationDTO>
    <Name>60i40e</Name>
    <Version>1803</Version>
    <ScoreRange>
      <Min>22</Min>
      <Max>30</Max>
    </ScoreRange>
    <Options>
      <AllocationOptionDTO>
        <Name>One Fund Option</Name>
        <OptionNumber>1</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>100</Percent>
            <FundCode>cig4243</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Two Fund Option</Name>
        <OptionNumber>2</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>60</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>40</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Three Fund Option</Name>
        <OptionNumber>3</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>40</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>50</Percent>
            <FundCode>maw104</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>10</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
    </Options>
  </AllocationDTO>
  <AllocationDTO>
    <Name>50i50e</Name>
    <Version>1803</Version>
    <ScoreRange>
      <Min>31</Min>
      <Max>59</Max>
    </ScoreRange>
    <Options>
      <AllocationOptionDTO>
        <Name>One Fund Option</Name>
        <OptionNumber>1</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>100</Percent>
            <FundCode>cig4244</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Two Fund Option</Name>
        <OptionNumber>2</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>50</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>50</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Three Fund Option</Name>
        <OptionNumber>3</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>40</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>35</Percent>
            <FundCode>maw104</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>25</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
    </Options>
  </AllocationDTO>
  <AllocationDTO>
    <Name>40i60e</Name>
    <Version>1803</Version>
    <ScoreRange>
      <Min>60</Min>
      <Max>87</Max>
    </ScoreRange>
    <Options>
      <AllocationOptionDTO>
        <Name>One Fund Option</Name>
        <OptionNumber>1</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>100</Percent>
            <FundCode>cig4245</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Two Fund Option</Name>
        <OptionNumber>2</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>40</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>60</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Three Fund Option</Name>
        <OptionNumber>3</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>30</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>30</Percent>
            <FundCode>maw104</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>40</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
    </Options>
  </AllocationDTO>
  <AllocationDTO>
    <Name>30i70e</Name>
    <Version>1803</Version>
    <ScoreRange>
      <Min>88</Min>
      <Max>114</Max>
    </ScoreRange>
    <Options>
      <AllocationOptionDTO>
        <Name>One Fund Option</Name>
        <OptionNumber>1</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>100</Percent>
            <FundCode>cig4246</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Two Fund Option</Name>
        <OptionNumber>2</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>30</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>70</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Three Fund Option</Name>
        <OptionNumber>3</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>20</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>25</Percent>
            <FundCode>maw104</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>55</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
    </Options>
  </AllocationDTO>
  <AllocationDTO>
    <Name>20i80e</Name>
    <Version>1803</Version>
    <ScoreRange>
      <Min>115</Min>
      <Max>135</Max>
    </ScoreRange>
    <Options>
      <AllocationOptionDTO>
        <Name>One Fund Option</Name>
        <OptionNumber>1</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>100</Percent>
            <FundCode>cig4247</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Two Fund Option</Name>
        <OptionNumber>2</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>20</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>80</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Three Fund Option</Name>
        <OptionNumber>3</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>10</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>20</Percent>
            <FundCode>maw104</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>70</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
    </Options>
  </AllocationDTO>
  <AllocationDTO>
    <Name>100e</Name>
    <Version>1803</Version>
    <ScoreRange>
      <Min>136</Min>
      <Max>150</Max>
    </ScoreRange>
    <Options>
      <AllocationOptionDTO>
        <Name>One Fund Option</Name>
        <OptionNumber>1</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>100</Percent>
            <FundCode>cig4248</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Two Fund Option</Name>
        <OptionNumber>2</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>0</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>100</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
      <AllocationOptionDTO>
        <Name>Three Fund Option</Name>
        <OptionNumber>3</OptionNumber>
        <CompositionParts>
          <CompositionPart>
            <Percent>0</Percent>
            <FundCode>pmo205</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>0</Percent>
            <FundCode>maw104</FundCode>
          </CompositionPart>
          <CompositionPart>
            <Percent>100</Percent>
            <FundCode>edg500</FundCode>
          </CompositionPart>
        </CompositionParts>
      </AllocationOptionDTO>
    </Options>
  </AllocationDTO>
</ArrayOfAllocationDTO>');

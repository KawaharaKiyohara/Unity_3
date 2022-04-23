Shader "VolumeLight/DrawVolume"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Blend OneMinusDstColor One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma enable_d3d11_debug_symbols
            #include "UnityCG.cginc"

            sampler2D volumeFrontTexture;
            sampler2D volumeBackTexture;
            float4x4 viewProjMatrixInv; // �r���[�v���W�F�N�V�����ϊ��̋t�s��B
            float ramdomSeed;           // �����_���V�[�h�B
            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 posInProj : TEXCOORD0;   // �ˉe��Ԃ̍��W�B
            };
            // �X�|�b�g���C�g
            struct SpotLight
            {
                float3 position;        // ���W
                int isUse;              // �g�p���t���O�B
                float3 positionInView;  // �J������Ԃł̍��W�B
                int no ;                // ���C�g�̔ԍ��B
                float3 direction;       // �ˏo�����B
                float range;            // �e���͈́B
                float3 color;           // ���C�g�̃J���[�B
                float3 color2;          // ��ڂ̃J���[�B
                float3 color3;          // �O�ڂ̃J���[�B
                float3 directionInView; // �J������Ԃł̎ˏo�����B
                float3 rangePow;        // �����ɂ����̉e�����ɗݏ悷��p�����[�^�[�B1.0�Ő��`�̕ω�������B
                                        // x����ڂ̃J���[�Ay����ڂ̃J���[�Az���O�ڂ̃J���[�B
                float3 angle;           // �ˏo�p�x(�P�ʁF���W�A���Bx����ڂ̃J���[�Ay����ڂ̃J���[�Az���O�ڂ̃J���[)�B
                float3 anglePow;        // �X�|�b�g���C�g�Ƃ̊p�x�ɂ����̉e�����ɗݏ悷��p�����[�^�B1.0�Ő��`�ɕω�����B
                                        // x����ڂ̃J���[�Ay����ڂ̃J���[�Az���O�ڂ̃J���[�B
            };
                        
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex ;
                
                o.vertex.z = 1.0f;
                o.vertex.w = 1.0f;
                o.posInProj = o.vertex;
                return o;
            }
            /*!
             * @brief	UV���W�Ɛ[�x�l���烏�[���h���W���v�Z����B
             * @param[in]	uv				uv���W
             * @param[in]	zInScreen		�X�N���[�����W�n�̐[�x�l
             * @param[in]	mViewProjInv	�r���[�v���W�F�N�V�����s��̋t�s��B
             */
            float3 CalcWorldPosFromUVZ(float2 uv, float zInScreen, float4x4 mViewProjInv)
            {
                float3 screenPos;
                screenPos.xy = (uv * float2(2.0f, -2.0f)) + float2(-1.0f, 1.0f);
                screenPos.z = zInScreen;

                float4 worldPos = mul(mViewProjInv, float4(screenPos, 1.0f));
                worldPos.xyz /= worldPos.w;
                return worldPos.xyz;
            }
            float GetRandomNumber(float2 texCoord, float Seed)
            {
                return frac(sin(dot(texCoord.xy, float2(12.9898, 78.233)) + Seed) * 43758.5453);
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.posInProj.xy;
                uv *= float2(0.5f, -0.5f);
                uv += 0.5f;
                
                // 
                float volumeFrontZ = tex2D(volumeFrontTexture, uv).r;
                float volumeBackZ = tex2D(volumeBackTexture, uv).r;
                
                float3 volumePosBack = CalcWorldPosFromUVZ(uv, volumeBackZ, viewProjMatrixInv);
                float3 volumePosFront = CalcWorldPosFromUVZ(uv, volumeFrontZ, viewProjMatrixInv);

                // todo �X�|�b�g���C�g�̕����͉��B��Ńe�N�X�`���ɏ����o���Bfloat t0 = dot(spotLight.direction, volumePosFront - spotLight.position);
                // todo �X�|�b�g���C�g�̕����͉��B��Ńe�N�X�`���ɏ����o���Bfloat t1 = dot(spotLight.direction, volumePosBack - spotLight.position);
                float t0 = dot(float3( 0.0f, 1.0f, 0.0f ), volumePosFront);
                float t1 = dot(float3(0.0f, 1.0f, 0.0f), volumePosBack );
                float t = t0 / (t0 + t1);
                float3 volumeCenterPos = lerp(volumePosFront, volumePosBack, t);
                float volume = length(volumePosBack - volumePosFront);

                // �{�����[�����Ȃ��ӏ��̓s�N�Z���L���B
                clip(volume - 0.001f);

                // float4 albedoColor = albedoTexture.Sample(Sampler, uv);
                float4 albedoColor = float4( 0.5f, 0.5f, 0.5f, 1.0f);
                // todo �����͂��ׂăf�[�^������������Ă���悤�ɂ���B
                SpotLight spotLight;
                spotLight.position = float3(0.0f, 0.0f, 0.0f);
                spotLight.angle = float3( 0.6f, 0.2f, 0.3f );
                spotLight.anglePow = float3(1.5f, 2.0f, 1.0f );
                spotLight.range = float3( 200.0f, 100.0f, 100.0f );
                spotLight.rangePow = float3(1.0f, 2.0f, 10.0f );
                spotLight.direction = float3( 0.0f, 1.0f, 0.0f);
                spotLight.color = float3( 10.0f, 0.0f, 0.0f);
                spotLight.color2 = float3( 50.0f, 50.0f, 0.0f);
                spotLight.color3 = float3( 200.0f, 200.0f, 200.0f);

                // �����ɂ����̉e�������v�Z�B
                float3 ligDir = (volumeCenterPos - spotLight.position);
                float distance = length(ligDir);
                ligDir = normalize(ligDir);
                float3 affectBase = 1.0f - min(1.0f, distance / spotLight.range);
                float3 affect = pow( affectBase, spotLight.rangePow);     

                // �����Ċp�x�ɂ�錸�����v�Z����B
                // �p�x�ɔ�Ⴕ�ď������Ȃ��Ă����e�������v�Z����
                float angleLigToPixel = saturate(dot(ligDir, spotLight.direction) );
                
                // dot()�ŋ��߂��l��acos()�ɓn���Ċp�x�����߂�
                angleLigToPixel = abs(acos(angleLigToPixel)) ;
                
                // ���̊p�x�ɂ�錸�����v�Z�B
                float3 angleAffectBase = max( 0.0f, 1.0f - 1.0f / spotLight.angle * angleLigToPixel );
                angleAffectBase = min( 1.0f, angleAffectBase * 1.8f);
                float3 angleAffect = pow( angleAffectBase, spotLight.anglePow );    
                affect *= angleAffect;

                float3 lig = 0;
                // �O�̌��������B    
                // ���̃x�[�X���v�Z�B
                float3 ligBase = albedoColor * step( volumeFrontZ, albedoColor.w ) * max( 0.0f, log(volume) ) * 0.1f;
                // ���̃x�[�X�ɉe��������Z����B
                lig = ligBase * affect.x * spotLight.color; 
                lig += ligBase * affect.y * spotLight.color2;
                lig += ligBase * affect.z * spotLight.color3;
                
                // ��C���̃`���̕\���Ƃ��ăm�C�Y��������B
                lig *= lerp( 0.9f, 1.1f, GetRandomNumber(uv, ramdomSeed));
                // lig *= lerp( 0.9f, 1.1f, 0.0f);

                return fixed4( lig, 1.0f);
            }
            ENDCG
        }
    }
}
